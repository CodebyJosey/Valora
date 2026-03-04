using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.ML.Data;

namespace PriceWise.Infrastructure.ML.Models;

/// <summary>
/// Represents one row in the laptop training dataset.
/// </summary>
public sealed class LaptopPriceTrainingRow
{
    [LoadColumn(0)]
    public string Brand { get; set; } = string.Empty;

    [LoadColumn(1)]
    public string Cpu { get; set; } = string.Empty;

    [LoadColumn(2)]
    public float RamGb { get; set; }

    [LoadColumn(3)]
    public float StorageGb { get; set; }

    [LoadColumn(4)]
    public string Gpu { get; set; } = string.Empty;

    // Label column
    [LoadColumn(5)]
    [ColumnName("Label")]
    public float Price { get; set; }
}