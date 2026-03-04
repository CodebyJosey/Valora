using Microsoft.AspNetCore.Mvc;
using PriceWise.Application.Interfaces;
using PriceWise.Domain.Entities;

namespace PriceWise.Api.Controllers;

[ApiController]
[Route("api/predictions")]
public class PredictionsController : ControllerBase
{
    private readonly IPricePredictor _predictor;

    public PredictionsController(IPricePredictor predictor)
    {
        _predictor = predictor;
    }

    [HttpPost("price")]
    public async Task<IActionResult> PredictPrice(ProductFeatures features)
    {
        float price = await _predictor.PredictPriceAsync(features);

        return Ok(new
        {
            predictor = _predictor.GetType().FullName
        });
    }
}