using Microsoft.ML;
using Microsoft.ML.Data;
using Microsoft.ML.Trainers.FastTree;
using PriceWise.Infrastructure.ML.Models;

namespace PriceWise.Infrastructure.ML.Training;

/// <summary>
/// Trains and saves a regression model for laptop price prediction.
/// </summary>
public sealed class LaptopPriceModelTrainer
{
    private readonly MLContext _ml = new(seed: 1);

    public RegressionTrainingResult TrainEvaluateAndSave(string csvPath, string modelPath)
    {
        if (!File.Exists(csvPath))
            throw new FileNotFoundException($"Dataset not found at '{csvPath}'.", csvPath);

        IDataView data = _ml.Data.LoadFromTextFile<LaptopPriceTrainingRow>(
            path: csvPath,
            hasHeader: true,
            separatorChar: ',');

        DataOperationsCatalog.TrainTestData split = _ml.Data.TrainTestSplit(data, testFraction: 0.2);

        // Robust categorical processing:
        // string -> key -> one-hot
        var pipeline =
            _ml.Transforms.Conversion.MapValueToKey("BrandKey", nameof(LaptopPriceTrainingRow.Brand))
            .Append(_ml.Transforms.Categorical.OneHotEncoding("BrandOneHot", "BrandKey"))

            .Append(_ml.Transforms.Conversion.MapValueToKey("CpuKey", nameof(LaptopPriceTrainingRow.Cpu)))
            .Append(_ml.Transforms.Categorical.OneHotEncoding("CpuOneHot", "CpuKey"))

            .Append(_ml.Transforms.Conversion.MapValueToKey("GpuKey", nameof(LaptopPriceTrainingRow.Gpu)))
            .Append(_ml.Transforms.Categorical.OneHotEncoding("GpuOneHot", "GpuKey"))

            .Append(_ml.Transforms.Concatenate("Features",
                "BrandOneHot",
                "CpuOneHot",
                "GpuOneHot",
                nameof(LaptopPriceTrainingRow.RamGb),
                nameof(LaptopPriceTrainingRow.StorageGb)))

            .Append(_ml.Regression.Trainers.FastTree(
                labelColumnName: "Label",
                featureColumnName: "Features"));

        // Fit on train set only
        ITransformer model = pipeline.Fit(split.TrainSet);

        // Evaluate on test set
        IDataView testPredictions = model.Transform(split.TestSet);
        RegressionMetrics metrics = _ml.Regression.Evaluate(testPredictions, labelColumnName: "Label");

        var cols = testPredictions.Schema.Select(c => new { c.Name, Type = c.Type.ToString() }).ToList();
        Console.WriteLine("=== PREDICTION OUTPUT COLUMNS ===");
        foreach (var c in cols)
        {
            Console.WriteLine($"{c.Name} : {c.Type}");
        }

        // ---- SANITY PREDICT (so we know immediately if it's working) ----
        PredictionEngine<LaptopPriceTrainingRow, LaptopPricePrediction>? sanityEngine = _ml.Model.CreatePredictionEngine<LaptopPriceTrainingRow, LaptopPricePrediction>(model);

        LaptopPriceTrainingRow? sanityRow = new LaptopPriceTrainingRow
        {
            Brand = "Dell",
            Cpu = "i7",
            RamGb = 16,
            StorageGb = 512,
            Gpu = "RTX3050",
            Price = 0
        };

        float sanityPrediction = sanityEngine.Predict(sanityRow).PredictedPrice;

        // Save model
        Directory.CreateDirectory(Path.GetDirectoryName(modelPath)!);
        using (FileStream? fs = File.Create(modelPath))
        {
            _ml.Model.Save(model, data.Schema, fs);
        }

        int rowCount = _ml.Data.CreateEnumerable<LaptopPriceTrainingRow>(data, reuseRowObject: false).Count();

        double r2 = metrics.RSquared;
        double? r2Safe = double.IsNaN(r2) || double.IsInfinity(r2) ? null : r2;

        return new RegressionTrainingResult(
            Rmse: metrics.RootMeanSquaredError,
            RSquared: r2Safe,
            RowCount: rowCount,
            ModelPath: modelPath,
            SanityPrediction: sanityPrediction);
    }
}