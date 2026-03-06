using System.Text.Json;
using PriceWise.Application.Abstractions.PricePrediction;

namespace PriceWise.Infrastructure.ML.Services;

/// <summary>
/// Stores training metadata as JSON files on disk, one file per category.
/// </summary>
public sealed class FilePriceTrainingMetadataStore : IPriceTrainingMetadataStore
{
    private readonly IPricePredictionCategoryRegistry _categoryRegistry;
    private readonly string _metadataDirectoryPath;
    private readonly JsonSerializerOptions _jsonOptions;
    private readonly object _syncRoot = new();

    /// <summary>
    /// Initializes a new instance of the <see cref="FilePriceTrainingMetadataStore"/> class.
    /// </summary>
    /// <param name="categoryRegistry">The category registry.</param>
    /// <param name="repoRootPath">The absolute repository root path.</param>
    public FilePriceTrainingMetadataStore(
        IPricePredictionCategoryRegistry categoryRegistry,
        string repoRootPath)
    {
        _categoryRegistry = categoryRegistry;
        _metadataDirectoryPath = Path.Combine(repoRootPath, "artifacts", "metadata");
        _jsonOptions = new JsonSerializerOptions
        {
            WriteIndented = true
        };
    }

    /// <inheritdoc />
    public void Save(PriceTrainingMetadata metadata)
    {
        ArgumentNullException.ThrowIfNull(metadata);

        lock (_syncRoot)
        {
            Directory.CreateDirectory(_metadataDirectoryPath);

            string filePath = GetFilePath(metadata.CategoryKey);
            string json = JsonSerializer.Serialize(metadata, _jsonOptions);

            File.WriteAllText(filePath, json);
        }
    }

    /// <inheritdoc />
    public bool TryGet(string categoryKey, out PriceTrainingMetadata? metadata)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(categoryKey);

        metadata = null;

        if (!_categoryRegistry.TryGet(categoryKey, out _))
        {
            return false;
        }

        lock (_syncRoot)
        {
            string filePath = GetFilePath(categoryKey);

            if (!File.Exists(filePath))
            {
                return false;
            }

            string json = File.ReadAllText(filePath);

            metadata = JsonSerializer.Deserialize<PriceTrainingMetadata>(json, _jsonOptions);
            return metadata is not null;
        }
    }

    private string GetFilePath(string categoryKey)
    {
        return Path.Combine(_metadataDirectoryPath, $"{categoryKey}.training.json");
    }
}