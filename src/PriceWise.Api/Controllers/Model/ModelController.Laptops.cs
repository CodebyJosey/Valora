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
    /// Trains a new ML model and saves it to artifacts/models/laptop-price-model.zip.
    /// Reloads the model in-memory afterwards so prediction uses the latest model without restarting.
    /// </summary>
    [HttpPost("laptops/train")]
    [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public IActionResult TrainLaptops()
    {
        string repoRoot = Directory.GetParent(_env.ContentRootPath)!.Parent!.FullName;

        string dataSetPath = Path.Combine(repoRoot, "data", "datasets", "laptops.csv");
        string modelPath = Path.Combine(repoRoot, "artifacts", "models", "laptop-price-model.zip");

        try
        {
            MLContext ml = new MLContext(seed: 1);
            LaptopPriceModelDefinition? definition = new LaptopPriceModelDefinition();
            TabularRegressionTrainer<LaptopPriceTrainingRow, LaptopPricePrediction>? trainer = new TabularRegressionTrainer<LaptopPriceTrainingRow, LaptopPricePrediction>(ml, definition);
            RegressionTrainingResult? result = trainer.TrainEvaluateAndSave(dataSetPath, modelPath);

            // Ensure the API starts using the freshly trained model immediately.
            _modelProvider.Reload();

            FileInfo? info = new FileInfo(modelPath);

            return Ok(new
            {
                message = "Model trained, saved and reloaded.",
                dataSetPath,
                modelPath,
                rowCount = result.RowCount,
                rmse = result.Rmse,
                rSquared = result.RSquared,
                sanityPrediction = result.SanityPrediction, // if you included it
                sizeBytes = info.Length,
                lastWriteUtc = info.LastWriteTimeUtc
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
    [HttpGet("laptops/peek")]
    public IActionResult PeekLaptops()
    {
        string repoRoot = Directory.GetParent(_env.ContentRootPath)!.Parent!.FullName;
        string csvPath = Path.Combine(repoRoot, "data", "datasets", "laptops.csv");

        MLContext? ml = new MLContext(seed: 1);

        // Keep separatorChar consistent with your dataset
        IDataView data = ml.Data.LoadFromTextFile<LaptopPriceTrainingRow>(
            path: csvPath,
            hasHeader: true,
            separatorChar: ',');

        List<LaptopPriceTrainingRow>? sample = ml.Data.CreateEnumerable<LaptopPriceTrainingRow>(data, reuseRowObject: false)
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
    [HttpPost("laptops/sanity-predict")]
    public IActionResult SanityPredictLaptops([FromBody] LaptopProductFeatures request)
    {
        if (request is null)
        {
            return BadRequest(Problem("Request body is required."));
        }

        string repoRoot = Directory.GetParent(_env.ContentRootPath)!.Parent!.FullName;
        string modelPath = Path.Combine(repoRoot, "artifacts", "models", "laptop-price-model.zip");

        if (!System.IO.File.Exists(modelPath))
        {
            return NotFound(new
            {
                message = "Laptop model not found.",
                modelPath
            });
        }

        MLContext ml = new MLContext(seed: 1);

        using FileStream fs = System.IO.File.OpenRead(modelPath);
        ITransformer model = ml.Model.Load(fs, out _);

        LaptopPriceTrainingRow input = new LaptopPriceTrainingRow
        {
            Brand = request.Brand.Trim(),
            Cpu = request.Cpu.Trim(),
            RamGb = request.RamGb,
            StorageGb = request.StorageGb,
            Gpu = request.Gpu.Trim().Replace(" ", ""),
            ScreenSizeInch = request.ScreenSizeInch,
            RefreshRate = request.RefreshRate,
            ReleaseYear = request.ReleaseYear,
            Condition = request.Condition.Trim(),
            Segment = request.Segment.Trim(),
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