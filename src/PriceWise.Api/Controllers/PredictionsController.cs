using Microsoft.AspNetCore.Mvc;
using PriceWise.Application.Interfaces;
using PriceWise.Domain.Entities;

namespace PriceWise.Api.Controllers;

[ApiController]
[Route("api/predictions")]
public sealed class PredictionsController : ControllerBase
{
    private readonly IPricePredictor _predictor;

    public PredictionsController(IPricePredictor predictor)
    {
        _predictor = predictor;
    }

    /// <summary>
    /// Predicts the price of a laptop based on the provided feature set.
    /// </summary>
    /// <param name="features">Laptop features used for prediction.</param>
    /// <returns>A predicted price (float) and some debug metadata.</returns>
    [HttpPost("price/laptops")]
    [ProducesResponseType(typeof(PredictionResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<PredictionResponse>> PredictLaptopPrice([FromBody] LaptopProductFeatures features)
    {
        if (features is null)
        {
            return BadRequest(Problem(title: "Invalid input", detail: "Request body is required."));
        }

        // Minimal validation (keep it simple)
        if (string.IsNullOrWhiteSpace(features.Brand) ||
            string.IsNullOrWhiteSpace(features.Cpu) ||
            string.IsNullOrWhiteSpace(features.Gpu))
        {
            return BadRequest(Problem(
                title: "Invalid input",
                detail: "brand, cpu and gpu are required."));
        }

        if (features.RamGb <= 0 || features.StorageGb <= 0)
        {
            return BadRequest(Problem(
                title: "Invalid input",
                detail: "ramGb and storageGb must be > 0."));
        }

        float predictedPrice = await _predictor.PredictPriceAsync(features);

        // optional: clamp negatives (some regressors can output negatives)
        if (predictedPrice < 0)
        {
            predictedPrice = 0;
        }

        PredictionResponse? response = new PredictionResponse
        {
            PredictedPrice = predictedPrice,
            Predictor = _predictor.GetType().FullName ?? _predictor.GetType().Name,
            Input = new LaptopProductFeatures
            {
                Brand = features.Brand,
                Cpu = features.Cpu,
                Gpu = features.Gpu,
                RamGb = features.RamGb,
                StorageGb = features.StorageGb
            }
        };

        return Ok(response);
    }

    /// <summary>
    /// Response returned by the prediction endpoint.
    /// </summary>
    public sealed class PredictionResponse
    {
        public float PredictedPrice { get; init; }
        public string Predictor { get; init; } = string.Empty;
        public LaptopProductFeatures Input { get; init; } = new();
    }
}