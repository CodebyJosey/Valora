using Valora.Application.Contracts.Admin;

namespace Valora.Application.Abstractions.Admin;

/// <summary>
/// Service for model performance monitoring.
/// </summary>
public interface IModelPerformanceService
{
    /// <summary>
    /// Gets the overall performance overview.
    /// </summary>
    Task<ModelPerformanceOverviewResponse> GetOverviewAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets performance for a single category.
    /// </summary>
    Task<ModelPerformanceCategoryResponse?> GetCategoryAsync(string categoryKey, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets recent listing-level performance rows.
    /// </summary>
    Task<IReadOnlyCollection<ModelPerformanceListingResponse>> GetRecentListingsAsync(CancellationToken cancellationToken = default);
}