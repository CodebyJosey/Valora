namespace PriceWise.DatasetGenerator.Models;

/// <summary>
/// Represents one generated laptop dataset row.
/// </summary>
public sealed class LaptopDatasetRow
{
    public string Brand { get; set; } = string.Empty;
    public string Cpu { get; set; } = string.Empty;
    public int RamGb { get; set; }
    public int StorageGb { get; set; }
    public string Gpu { get; set; } = string.Empty;
    public float ScreenSizeInch { get; set; }
    public int RefreshRate { get; set; }
    public int ReleaseYear { get; set; }
    public string Condition { get; set; } = string.Empty;
    public string Segment { get; set; } = string.Empty;
    public decimal Price { get; set; }
}