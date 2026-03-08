namespace Valora.Application.Contracts.Listings;

/// <summary>
/// Request for marking a listing as sold.
/// </summary>
public sealed class MarkListingAsSoldRequest
{
    /// <summary>
    /// Gets or sets the sold price.
    /// </summary>
    public decimal SoldPrice { get; set; }
}