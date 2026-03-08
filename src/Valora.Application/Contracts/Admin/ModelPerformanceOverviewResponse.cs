namespace Valora.Application.Contracts.Admin;

/// <summary>
/// Overall model performance overview.
/// </summary>
public sealed class ModelPerformanceOverviewResponse
{
    /// <summary>
    /// Gets or sets the total sold listings.
    /// </summary>
    public int TotalSoldListings { get; init; }

    /// <summary>
    /// Gets or sets the sold listings that also had a predicted price.
    /// </summary>
    public int SoldListingsWithPrediction { get; init; }

    /// <summary>
    /// Gets or sets the average absolute error.
    /// </summary>
    public decimal AverageAbsoluteError { get; init; }

    /// <summary>
    /// Gets or sets the average percentage error.
    /// </summary>
    public decimal AveragePercentageError { get; init; }

    /// <summary>
    /// Gets or sets category performance items.
    /// </summary>
    public required IReadOnlyCollection<ModelPerformanceCategoryResponse> Categories { get; init; }
}