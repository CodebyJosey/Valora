using Microsoft.ML;
using PriceWise.Application.Interfaces;
using PriceWise.Domain.Entities;
using PriceWise.Infrastructure.ML.Models;

namespace PriceWise.Infrastructure.ML.Prediction;

/// <summary>
/// ML.NET based price predictor using a loaded model.
/// A PredictionEngine is created per request/call (thread-safe for ASP.NET Core).
/// </summary>
public sealed class MlNetPricePredictor : IPricePredictor
{
    private readonly MLContext _ml;
    private readonly ITransformer _model;

    /// <summary>
    /// Creates a predictor using a shared MLContext and a loaded model.
    /// </summary>
    public MlNetPricePredictor(MLContext ml, ITransformer model)
    {
        _ml = ml;
        _model = model;
    }

    public Task<float> PredictPriceAsync(ProductFeatures features)
    {
        LaptopPriceTrainingRow? input = new LaptopPriceTrainingRow
        {
            Brand = features.Brand,
            Cpu = features.Cpu,
            Gpu = features.Gpu,
            RamGb = features.RamGb,
            StorageGb = features.StorageGb,
            Price = 0 // label not used during prediction
        };

        // PredictionEngine is NOT thread-safe, so create it per call.
        PredictionEngine<LaptopPriceTrainingRow, LaptopPricePrediction>? engine = _ml.Model.CreatePredictionEngine<LaptopPriceTrainingRow, LaptopPricePrediction>(_model);
        LaptopPricePrediction prediction = engine.Predict(input);

        return Task.FromResult(prediction.PredictedPrice);
    }
}