namespace PriceWise.DatasetGenerator.Models;

/// <summary>
/// Represents one generated phone dataset row.
/// </summary>
public sealed class PhoneDatasetRow
{
    /// <summary>
    /// Gets or sets the phone brand.
    /// </summary>
    public string Brand { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the phone model family.
    /// Examples: iPhone13, GalaxyS23, Pixel8.
    /// </summary>
    public string ModelFamily { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the storage size in GB.
    /// </summary>
    public int StorageGb { get; set; }

    /// <summary>
    /// Gets or sets the RAM size in GB.
    /// </summary>
    public int RamGb { get; set; }

    /// <summary>
    /// Gets or sets the battery health percentage.
    /// </summary>
    public int BatteryHealth { get; set; }

    /// <summary>
    /// Gets or sets the condition category.
    /// </summary>
    public string Condition { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the release year.
    /// </summary>
    public int ReleaseYear { get; set; }

    /// <summary>
    /// Gets or sets the generated phone price.
    /// </summary>
    public decimal Price { get; set; }
}