using Microsoft.AspNetCore.Mvc;
using Microsoft.ML;
using PriceWise.Domain.Entities;
using PriceWise.Infrastructure.ML.Definitions;
using PriceWise.Infrastructure.ML.Models;
using PriceWise.Infrastructure.ML.Training;

namespace PriceWise.Api.Controllers;

public partial class ModelController : ControllerBase
{
    /// <summary>
    /// Trains a new ML model and saves it to artifacts/models/phones-price-model.zip.
    /// Reloads the model in-memory afterwards so prediction uses the latest model without restarting.
    /// </summary>
    [HttpPost("phones/train")]
    [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public IActionResult TrainPhones()
    {
        string repoRoot = Directory.GetParent(_env.ContentRootPath)!.Parent!.FullName;

        string dataSetPath = Path.Combine(repoRoot, "data", "datasets", "phones.csv");
        string modelPath = Path.Combine(repoRoot, "artifacts", "models", "phone-price-model.zip");

        try
        {
            MLContext ml = new MLContext(seed: 1);
            PhonePriceModelDefinition? definition = new PhonePriceModelDefinition();
            TabularRegressionTrainer<PhonePriceTrainingRow, PhonePricePrediction>? trainer = new TabularRegressionTrainer<PhonePriceTrainingRow, PhonePricePrediction>(ml, definition);
            RegressionTrainingResult? result = trainer.TrainEvaluateAndSave(dataSetPath, modelPath);

            // Ensure the API starts using the freshly trained model immediately.
            _modelProvider.Reload();

            FileInfo? info = new FileInfo(modelPath);

            return Ok(new
            {
                message = "Phone model trained and saved.",
                dataSetPath,
                modelPath,
                rowCount = result.RowCount,
                rmse = result.Rmse,
                rSquared = result.RSquared,
                sanityPrediction = result.SanityPrediction
            });
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError,
                Problem(title: "Training failed", detail: ex.Message));
        }
    }

    /// <summary>
    /// Quick check: load first few rows from the dataset as ML.NET sees them.
    /// Helps debug CSV delimiter/column mapping issues.
    /// </summary>
    [HttpGet("phones/peek")]
    public IActionResult PeekPhones()
    {
        string repoRoot = Directory.GetParent(_env.ContentRootPath)!.Parent!.FullName;
        string csvPath = Path.Combine(repoRoot, "data", "datasets", "phones.csv");

        MLContext? ml = new MLContext(seed: 1);

        // Keep separatorChar consistent with your dataset
        IDataView data = ml.Data.LoadFromTextFile<PhonePriceTrainingRow>(
            path: csvPath,
            hasHeader: true,
            separatorChar: ',');

        List<PhonePriceTrainingRow>? sample = ml.Data.CreateEnumerable<PhonePriceTrainingRow>(data, reuseRowObject: false)
            .Take(5)
            .ToList();

        return Ok(new
        {
            csvPath,
            sample
        });
    }

    /// <summary>
    /// Sanity prediction endpoint that uses the currently loaded model (via ITrainedModelProvider)
    /// and reads the Score directly from an IDataView (very reliable).
    /// </summary>
    [HttpPost("phones/sanity-predict")]
    public IActionResult SanityPredictPhones([FromBody] PhoneProductFeatures request)
    {
        if (request is null)
        {
            return BadRequest(Problem("Request body is required."));
        }

        string repoRoot = Directory.GetParent(_env.ContentRootPath)!.Parent!.FullName;
        string modelPath = Path.Combine(repoRoot, "artifacts", "models", "phone-price-model.zip");

        if (!System.IO.File.Exists(modelPath))
        {
            return NotFound(new
            {
                message = "Phone model not found.",
                modelPath
            });
        }

        MLContext ml = new MLContext(seed: 1);

        using FileStream fs = System.IO.File.OpenRead(modelPath);
        ITransformer model = ml.Model.Load(fs, out _);

        PhonePriceTrainingRow input = new PhonePriceTrainingRow
        {
            Brand = request.Brand.Trim(),
            ModelFamily = request.ModelFamily.Trim(),
            StorageGb = request.StorageGb,
            RamGb = request.RamGb,
            BatteryHealth = request.BatteryHealth,
            Condition = request.Condition.Trim(),
            ReleaseYear = request.ReleaseYear,
            Price = 0
        };

        IDataView inputView = ml.Data.LoadFromEnumerable(new[] { input });
        IDataView scored = model.Transform(inputView);

        float score = ml.Data.CreateEnumerable<ScoreRow>(scored, reuseRowObject: false)
            .First()
            .Score;

        return Ok(new
        {
            input,
            predictedPrice = score
        });
    }
}