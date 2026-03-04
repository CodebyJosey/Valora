using Microsoft.AspNetCore.Mvc;
using Microsoft.ML;
using PriceWise.Infrastructure.ML.Models;
using PriceWise.Infrastructure.ML.Training;

namespace PriceWise.Api.Controllers;

[ApiController]
[Route("api/model")]
public class ModelController : ControllerBase
{
    private readonly IHostEnvironment _env;

    public ModelController(IHostEnvironment env)
    {
        _env = env;
    }

    [HttpPost("train")]
    public IActionResult Train()
    {
        string repoRoot = Directory.GetParent(_env.ContentRootPath)!.Parent!.FullName;

        string dataSetPath = Path.Combine(repoRoot, "data", "datasets", "laptops.csv");
        string modelPath = Path.Combine(repoRoot, "artifacts", "models", "laptop-price-model.zip");

        LaptopPriceModelTrainer? trainer = new LaptopPriceModelTrainer();
        RegressionTrainingResult? result = trainer.TrainEvaluateAndSave(dataSetPath, modelPath);

        FileInfo? info = new FileInfo(modelPath);

        return Ok(new
        {
            message = "Model trained and saved.",
            dataSetPath,
            modelPath,
            rowCount = result.RowCount,
            rmse = result.Rmse,
            rSquared = result.RSquared,
            sizeBytes = info.Length,
            lastWriteUtc = info.LastWriteTimeUtc,
            sanityPrediction = result.SanityPrediction
        });
    }

    [HttpPost("sanity-predict")]
    public IActionResult SanityPredict([FromBody] PredictPriceRequest request)
    {
        string repoRoot = Directory.GetParent(_env.ContentRootPath)!.Parent!.FullName;
        string modelPath = Path.Combine(repoRoot, "artifacts", "models", "laptop-price-model.zip");

        if (!System.IO.File.Exists(modelPath))
        {
            return NotFound(new { message = "Model not found", modelPath });
        }

        MLContext? ml = new MLContext(seed: 1);

        using FileStream? fs = System.IO.File.OpenRead(modelPath);
        ITransformer? model = ml.Model.Load(fs, out _);

        var engine = ml.Model.CreatePredictionEngine<LaptopPriceTrainingRow, LaptopPricePrediction>(model);

        var input = new LaptopPriceTrainingRow
        {
            Brand = request.Brand.Trim(),
            Cpu = request.Cpu.Trim(),
            RamGb = request.RamGb,
            StorageGb = request.StorageGb,
            Gpu = request.Gpu.Trim().Replace(" ", ""),
            Price = 0
        };

        var prediction = engine.Predict(input);

        return Ok(new
        {
            input,
            predictedPrice = prediction.PredictedPrice,
            modelPath
        });
    }

    [HttpGet("peek")]
    public IActionResult Peek()
    {
        string repoRoot = Directory.GetParent(_env.ContentRootPath)!.Parent!.FullName;
        string csvPath = Path.Combine(repoRoot, "data", "datasets", "laptops.csv");

        var ml = new MLContext(seed: 1);

        // ⚠️ Zet separatorChar hier DESNOODS op ';' als je CSV dat gebruikt
        var data = ml.Data.LoadFromTextFile<LaptopPriceTrainingRow>(
            path: csvPath,
            hasHeader: true,
            separatorChar: ',');

        var rows = ml.Data.CreateEnumerable<LaptopPriceTrainingRow>(data, reuseRowObject: false)
            .Take(5)
            .ToList();

        return Ok(new
        {
            csvPath,
            sample = rows
        });
    }
}

public sealed record PredictPriceRequest(
    string Brand,
    string Cpu,
    float RamGb,
    float StorageGb,
    string Gpu);