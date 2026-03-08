namespace Valora.Application.Contracts.Admin;

/// <summary>
/// Performance information for a single category.
/// </summary>
public sealed class ModelPerformanceCategoryResponse
{
    /// <summary>
    /// Gets or sets the category key.
    /// </summary>
    public required string CategoryKey { get; init; }

    /// <summary>
    /// Gets or sets the sold listing count.
    /// </summary>
    public int SoldListings { get; init; }

    /// <summary>
    /// Gets or sets the sold listings with prediction.
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
}