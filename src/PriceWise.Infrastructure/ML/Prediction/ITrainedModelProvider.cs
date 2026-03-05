using Microsoft.ML;

namespace PriceWise.Infrastructure.ML.Prediction;

public interface ITrainedModelProvider
{
    ITransformer GetModel();
    void Reload();
}