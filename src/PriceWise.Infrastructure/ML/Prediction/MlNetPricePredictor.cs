using Microsoft.ML;
using PriceWise.Application.Interfaces;
using PriceWise.Domain.Entities;
using PriceWise.Infrastructure.ML.Models;

namespace PriceWise.Infrastructure.ML.Prediction;

/// <summary>
/// ML.NET based price predictor using a loaded model.
/// A PredictionEngine is created per request/call (thread-safe for ASP.NET Core).
/// </summary>
public sealed class MlNetPricePredictor : IPricePredictor
{
    private readonly MLContext _ml;
    private readonly ITrainedModelProvider _modelProvider;

    /// <summary>
    /// Creates a predictor using a shared MLContext and a loaded model.
    /// </summary>
    public MlNetPricePredictor(MLContext ml, ITrainedModelProvider modelProvider)
    {
        _ml = ml;
        _modelProvider = modelProvider;
    }

    public Task<float> PredictPriceAsync(ProductFeatures features)
    {
        ITransformer model = _modelProvider.GetModel();

        LaptopPriceTrainingRow inputRow = new LaptopPriceTrainingRow
        {
            Brand = (features.Brand ?? "").Trim(),
            Cpu = (features.Cpu ?? "").Trim(),
            Gpu = (features.Gpu ?? "").Trim().Replace(" ", ""),
            RamGb = features.RamGb,
            StorageGb = features.StorageGb,
            Price = 0
        };

        IDataView inputView = _ml.Data.LoadFromEnumerable(new[]
        {
            inputRow
        });

        IDataView scoredView = model.Transform(inputView);

        float score = _ml.Data.CreateEnumerable<ScoreRow>(scoredView, reuseRowObject: false).First().Score;
        return Task.FromResult(score);
    }

    private sealed class ScoreRow
    {
        public float Score { get; set; }
    }
}