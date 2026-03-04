using FluentAssertions;
using PriceWise.Domain.Entities;
using PriceWise.Infrastructure.ML;

namespace PriceWise.Tests;

public class DummyPricePredictorTests
{
    [Fact]
    public async Task PredictPrice_ShouldReturnPositivePrice()
    {
        DummyPricePredictor predictor = new DummyPricePredictor();

        ProductFeatures features = new ProductFeatures
        {
            Brand = "Dell",
            Cpu = "i7",
            RamGb = 16,
            StorageGb = 512,
            Gpu = "RTX3060"
        };

        float result = await predictor.PredictPriceAsync(features);

        result.Should().BeGreaterThan(0);
    }
}
