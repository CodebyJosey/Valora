using Microsoft.ML;
using PriceWise.Application.Interfaces;
using PriceWise.Domain.Entities;
using PriceWise.Infrastructure.ML.Models;

namespace PriceWise.Infrastructure.ML.Prediction;

public sealed class MlNetPricePredictor : IPricePredictor
{
    private readonly MLContext _ml;
    private readonly ITrainedModelProvider _modelProvider;

    public MlNetPricePredictor(MLContext ml, ITrainedModelProvider modelProvider)
    {
        _ml = ml;
        _modelProvider = modelProvider;
    }

    public Task<float> PredictPriceAsync(LaptopProductFeatures features)
    {
        ITransformer? model = _modelProvider.GetModel();

        LaptopPriceTrainingRow? row = new LaptopPriceTrainingRow
        {
            Brand = (features.Brand ?? string.Empty).Trim(),
            Cpu = (features.Cpu ?? string.Empty).Trim(),
            Gpu = (features.Gpu ?? string.Empty).Trim().Replace(" ", ""),
            Condition = (features.Condition ?? string.Empty).Trim(),
            Segment = (features.Segment ?? string.Empty).Trim(),
            RamGb = features.RamGb,
            StorageGb = features.StorageGb,
            ScreenSizeInch = features.ScreenSizeInch,
            RefreshRate = features.RefreshRate,
            ReleaseYear = features.ReleaseYear,
            Price = 0
        };

        IDataView inputView = _ml.Data.LoadFromEnumerable(new[] { row });
        IDataView scoredView = model.Transform(inputView);

        float score = _ml.Data.CreateEnumerable<ScoreRow>(scoredView, reuseRowObject: false)
            .First()
            .Score;

        return Task.FromResult(score);
    }

    public Task<float> PredictPriceAsync(PhoneProductFeatures features)
    {
        ITransformer? model = _modelProvider.GetModel();

        PhonePriceTrainingRow? row = new PhonePriceTrainingRow
        {
            Brand = (features.Brand ?? string.Empty).Trim(),
            ModelFamily = (features.ModelFamily ?? string.Empty).Trim(),
            StorageGb = features.StorageGb,
            RamGb = features.RamGb,
            BatteryHealth = features.BatteryHealth,
            Condition = (features.Condition ?? string.Empty).Trim(),
            ReleaseYear = features.ReleaseYear,
            Price = 0
        };

        IDataView inputView = _ml.Data.LoadFromEnumerable(new[] { row });
        IDataView scoredView = model.Transform(inputView);

        float score = _ml.Data.CreateEnumerable<ScoreRow>(scoredView, reuseRowObject: false)
            .First()
            .Score;

        return Task.FromResult(score);
    }

    private sealed class ScoreRow
    {
        public float Score { get; set; }
    }
}