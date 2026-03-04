namespace PriceWise.Infrastructure.ML.Training;

/// <summary>
/// Training result metrics for a regression model.
/// </summary>
public sealed record RegressionTrainingResult(
    double Rmse,
    double? RSquared,
    int RowCount,
    string ModelPath,
    float SanityPrediction
);