using Microsoft.AspNetCore.Mvc;
using PriceWise.Infrastructure.ML.Prediction;

namespace PriceWise.Api.Controllers;

[ApiController]
[Route("api/model")]
public partial class ModelController : ControllerBase
{
    private readonly IHostEnvironment _env;
    private readonly ITrainedModelProvider _modelProvider;

    public ModelController(IHostEnvironment env, ITrainedModelProvider modelProvider)
    {
        _env = env;
        _modelProvider = modelProvider;
    }

    private sealed class ScoreRow
    {
        public float Score { get; set; }
    }
}
