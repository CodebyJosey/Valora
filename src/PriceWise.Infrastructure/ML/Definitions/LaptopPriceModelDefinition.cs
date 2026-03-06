using Microsoft.ML;
using Microsoft.ML.Trainers.FastTree;
using PriceWise.Infrastructure.ML.Abstractions;
using PriceWise.Infrastructure.ML.Models;

namespace PriceWise.Infrastructure.ML.Definitions;

/// <summary>
/// Defines the ML.NET pipeline and sanity sample for laptop price prediction.
/// </summary>
public sealed class LaptopPriceModelDefinition
    : ITabularRegressionDefinition<LaptopPriceTrainingRow, LaptopPricePrediction>
{
    public string ModelName => "laptop-price";

    public char SeparatorChar => ',';

    public IEstimator<ITransformer> BuildTrainingPipeline(MLContext ml)
    {
        return ml.Transforms.Text.NormalizeText("BrandNorm", nameof(LaptopPriceTrainingRow.Brand))
            .Append(ml.Transforms.Text.NormalizeText("CpuNorm", nameof(LaptopPriceTrainingRow.Cpu)))
            .Append(ml.Transforms.Text.NormalizeText("GpuNorm", nameof(LaptopPriceTrainingRow.Gpu)))
            .Append(ml.Transforms.Text.NormalizeText("ConditionNorm", nameof(LaptopPriceTrainingRow.Condition)))
            .Append(ml.Transforms.Text.NormalizeText("SegmentNorm", nameof(LaptopPriceTrainingRow.Segment)))

            .Append(ml.Transforms.Conversion.MapValueToKey("BrandKey", "BrandNorm"))
            .Append(ml.Transforms.Categorical.OneHotEncoding("BrandVec", "BrandKey"))

            .Append(ml.Transforms.Conversion.MapValueToKey("CpuKey", "CpuNorm"))
            .Append(ml.Transforms.Categorical.OneHotEncoding("CpuVec", "CpuKey"))

            .Append(ml.Transforms.Conversion.MapValueToKey("GpuKey", "GpuNorm"))
            .Append(ml.Transforms.Categorical.OneHotEncoding("GpuVec", "GpuKey"))

            .Append(ml.Transforms.Conversion.MapValueToKey("ConditionKey", "ConditionNorm"))
            .Append(ml.Transforms.Categorical.OneHotEncoding("ConditionVec", "ConditionKey"))

            .Append(ml.Transforms.Conversion.MapValueToKey("SegmentKey", "SegmentNorm"))
            .Append(ml.Transforms.Categorical.OneHotEncoding("SegmentVec", "SegmentKey"))

            .Append(ml.Transforms.Concatenate(
                "Features",
                "BrandVec",
                "CpuVec",
                "GpuVec",
                "ConditionVec",
                "SegmentVec",
                nameof(LaptopPriceTrainingRow.RamGb),
                nameof(LaptopPriceTrainingRow.StorageGb),
                nameof(LaptopPriceTrainingRow.ScreenSizeInch),
                nameof(LaptopPriceTrainingRow.RefreshRate),
                nameof(LaptopPriceTrainingRow.ReleaseYear)))

            .Append(ml.Regression.Trainers.FastTree(
                labelColumnName: "Label",
                featureColumnName: "Features",
                numberOfLeaves: 20,
                numberOfTrees: 100,
                minimumExampleCountPerLeaf: 2));
    }

    public LaptopPriceTrainingRow CreateSanitySample()
    {
        return new LaptopPriceTrainingRow
        {
            Brand = "Lenovo",
            Cpu = "i5",
            RamGb = 16,
            StorageGb = 512,
            Gpu = "Integrated",
            ScreenSizeInch = 15.6f,
            RefreshRate = 60,
            ReleaseYear = DateTime.UtcNow.Year - 1,
            Condition = "New",
            Segment = "Mainstream",
            Price = 0
        };
    }
}