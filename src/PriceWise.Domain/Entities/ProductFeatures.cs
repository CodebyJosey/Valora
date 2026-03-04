namespace PriceWise.Domain.Entities;

/// <summary>
/// Represents the features of a product used for price prediction.
/// </summary>
public class ProductFeatures
{
    /// <summary>
    /// Brand of the product.
    /// </summary>
    public string Brand { get; set; } = string.Empty;

    /// <summary>
    /// CPU model.
    /// </summary>
    public string Cpu { get; set; } = string.Empty;

    /// <summary>
    /// RAM in GB.
    /// </summary>
    public float RamGb { get; set; }

    /// <summary>
    /// Storage in GB.
    /// </summary>
    public float StorageGb { get; set; }

    /// <summary>
    /// GPU model.
    /// </summary>
    public string Gpu { get; set; } = string.Empty;
}