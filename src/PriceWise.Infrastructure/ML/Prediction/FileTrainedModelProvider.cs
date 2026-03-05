using Microsoft.ML;

namespace PriceWise.Infrastructure.ML.Prediction;

public sealed class FileTrainedModelProvider : ITrainedModelProvider
{
    private readonly MLContext _ml;
    private readonly string _modelPath;

    private readonly object _lock = new();
    private ITransformer? _model;

    public FileTrainedModelProvider(MLContext ml, string modelPath)
    {
        _ml = ml;
        _modelPath = modelPath;
    }

    public ITransformer GetModel()
    {
        lock (_lock)
        {
            _model ??= Load();
            return _model;
        }
    }

    public void Reload()
    {
        lock (_lock)
        {
            _model = Load();
        }
    }

    private ITransformer Load()
    {
        if (!File.Exists(_modelPath))
        {
            throw new FileNotFoundException($"Model not found at '{_modelPath}'. Train it first.");
        }

        using FileStream? fs = File.OpenRead(_modelPath);
        return _ml.Model.Load(fs, out _);
    }
}