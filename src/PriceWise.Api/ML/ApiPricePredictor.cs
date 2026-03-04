using Microsoft.Extensions.ML;
using PriceWise.Application.Interfaces;
using PriceWise.Domain.Entities;
using PriceWise.Infrastructure.ML.Models;

namespace PriceWise.Api.ML;

public sealed class ApiPricePredictor : IPricePredictor
{
    private const string ModelName = "LaptopPriceModel";
    private readonly PredictionEnginePool<LaptopPriceTrainingRow, LaptopPricePrediction> _pool;

    public ApiPricePredictor(PredictionEnginePool<LaptopPriceTrainingRow, LaptopPricePrediction> pool)
    {
        _pool = pool;
    }

    public Task<float> PredictPriceAsync(ProductFeatures features)
    {
        var input = new LaptopPriceTrainingRow
        {
            Brand = features.Brand,
            Cpu = features.Cpu,
            Gpu = features.Gpu,
            RamGb = features.RamGb,
            StorageGb = features.StorageGb,
            Price = 0
        };

        var prediction = _pool.Predict(ModelName, input);
        return Task.FromResult(prediction.PredictedPrice);
    }
}