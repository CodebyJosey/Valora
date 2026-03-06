using Microsoft.ML;
using PriceWise.Infrastructure.ML.Abstractions;
using PriceWise.Infrastructure.ML.Models;

namespace PriceWise.Infrastructure.ML.Definitions;

/// <summary>
/// Defines the ML.NET pipeline and sanity sample for phone price prediction.
/// </summary>
public sealed class PhonePriceModelDefinition
    : ITabularRegressionDefinition<PhonePriceTrainingRow, PhonePricePrediction>
{
    public string ModelName => "phone-price";

    public char SeparatorChar => ',';

    public IEstimator<ITransformer> BuildTrainingPipeline(MLContext ml)
    {
        return ml.Transforms.Text.NormalizeText("BrandNorm", nameof(PhonePriceTrainingRow.Brand))
            .Append(ml.Transforms.Text.NormalizeText("ModelFamilyNorm", nameof(PhonePriceTrainingRow.ModelFamily)))
            .Append(ml.Transforms.Text.NormalizeText("ConditionNorm", nameof(PhonePriceTrainingRow.Condition)))

            .Append(ml.Transforms.Conversion.MapValueToKey("BrandKey", "BrandNorm"))
            .Append(ml.Transforms.Categorical.OneHotEncoding("BrandVec", "BrandKey"))

            .Append(ml.Transforms.Conversion.MapValueToKey("ModelFamilyKey", "ModelFamilyNorm"))
            .Append(ml.Transforms.Categorical.OneHotEncoding("ModelFamilyVec", "ModelFamilyKey"))

            .Append(ml.Transforms.Conversion.MapValueToKey("ConditionKey", "ConditionNorm"))
            .Append(ml.Transforms.Categorical.OneHotEncoding("ConditionVec", "ConditionKey"))

            .Append(ml.Transforms.Concatenate(
                "Features",
                "BrandVec",
                "ModelFamilyVec",
                "ConditionVec",
                nameof(PhonePriceTrainingRow.StorageGb),
                nameof(PhonePriceTrainingRow.RamGb),
                nameof(PhonePriceTrainingRow.BatteryHealth),
                nameof(PhonePriceTrainingRow.ReleaseYear)))

            .Append(ml.Regression.Trainers.FastTree(
                labelColumnName: "Label",
                featureColumnName: "Features",
                numberOfLeaves: 20,
                numberOfTrees: 100,
                minimumExampleCountPerLeaf: 2));
    }

    public PhonePriceTrainingRow CreateSanitySample()
    {
        return new PhonePriceTrainingRow
        {
            Brand = "Apple",
            ModelFamily = "iPhone13",
            StorageGb = 128,
            RamGb = 4,
            BatteryHealth = 92,
            Condition = "UsedGood",
            ReleaseYear = 2021,
            Price = 0
        };
    }
}