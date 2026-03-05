using Microsoft.ML.Data;

namespace PriceWise.Infrastructure.ML.Models;

/// <summary>
/// ML.NET prediction output model.
/// </summary>
public sealed class LaptopPricePrediction
{
    [ColumnName("Score")]
    public float PredictedPrice { get; set; }
}