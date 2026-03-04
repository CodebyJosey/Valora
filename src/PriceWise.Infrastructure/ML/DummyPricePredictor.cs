using PriceWise.Application.Interfaces;
using PriceWise.Domain.Entities;

namespace PriceWise.Infrastructure.ML;

/// <summary>
/// Dummy implementation of the price predictor.
/// Will later be replaced with ML.NET.
/// </summary>
public class DummyPricePredictor : IPricePredictor
{
    public Task<float> PredictPriceAsync(ProductFeatures features)
    {
        float basePrice = 500;

        basePrice += features.RamGb * 20;
        basePrice += features.StorageGb * 0.5f;

        return Task.FromResult(basePrice);
    }
}