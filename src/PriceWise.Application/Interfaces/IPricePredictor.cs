using PriceWise.Domain.Entities;

namespace PriceWise.Application.Interfaces;

/// <summary>
/// Predicts the fair price of a product.
/// </summary>
public interface IPricePredictor
{
    Task<float> PredictPriceAsync(ProductFeatures features);
}