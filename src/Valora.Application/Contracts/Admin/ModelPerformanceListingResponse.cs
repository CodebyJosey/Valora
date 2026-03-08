namespace Valora.Application.Contracts.Admin;

/// <summary>
/// Listing-level model performance information.
/// </summary>
public sealed class ModelPerformanceListingResponse
{
    /// <summary>
    /// Gets or sets the listing id.
    /// </summary>
    public Guid ListingId { get; init; }

    /// <summary>
    /// Gets or sets the category key.
    /// </summary>
    public required string CategoryKey { get; init; }

    /// <summary>
    /// Gets or sets the listing title.
    /// </summary>
    public required string Title { get; init; }

    /// <summary>
    /// Gets or sets the asking price.
    /// </summary>
    public decimal AskingPrice { get; init; }

    /// <summary>
    /// Gets or sets the predicted price.
    /// </summary>
    public decimal PredictedPrice { get; init; }

    /// <summary>
    /// Gets or sets the sold price.
    /// </summary>
    public decimal SoldPrice { get; init; }

    /// <summary>
    /// Gets or sets the absolute error.
    /// </summary>
    public decimal AbsoluteError { get; init; }

    /// <summary>
    /// Gets or sets the percentage error.
    /// </summary>
    public decimal PercentageError { get; init; }

    /// <summary>
    /// Gets or sets the sold timestamp.
    /// </summary>
    public DateTime SoldAtUtc { get; init; }
}