namespace PriceWise.Application.Abstractions.PricePrediction;

/// <summary>
/// Stores and retrieves persisted training metadata per category.
/// </summary>
public interface IPriceTrainingMetadataStore
{
    /// <summary>
    /// Saves the latest training metadata for the specified category.
    /// </summary>
    /// <param name="metadata">The metadata to save.</param>
    void Save(PriceTrainingMetadata metadata);

    /// <summary>
    /// Attempts to get the latest training metadata for the specified category.
    /// </summary>
    /// <param name="categoryKey">The category key.</param>
    /// <param name="metadata">The resolved metadata.</param>
    /// <returns>True when found; otherwise false.</returns>
    bool TryGet(string categoryKey, out PriceTrainingMetadata? metadata);
}