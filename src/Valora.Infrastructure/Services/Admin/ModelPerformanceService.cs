using Microsoft.EntityFrameworkCore;
using Valora.Application.Abstractions.Admin;
using Valora.Application.Contracts.Admin;
using Valora.Domain.Constants;
using Valora.Infrastructure.Persistence;

namespace Valora.Infrastructure.Services.Admin;

/// <summary>
/// Database-backed model performance service.
/// </summary>
public sealed class ModelPerformanceService : IModelPerformanceService
{
    private readonly ValoraDbContext _dbContext;

    /// <summary>
    /// Initializes a new instance of the <see cref="ModelPerformanceService"/> class.
    /// </summary>
    public ModelPerformanceService(ValoraDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    /// <inheritdoc />
    public async Task<ModelPerformanceOverviewResponse> GetOverviewAsync(CancellationToken cancellationToken = default)
    {
        List<PerformanceRow> rows = await QueryPerformanceRows()
            .ToListAsync(cancellationToken);

        int totalSoldListings = await _dbContext.Listings
            .AsNoTracking()
            .CountAsync(x => x.Status == ListingStatuses.Sold, cancellationToken);

        int soldListingsWithPrediction = rows.Count;

        decimal avgAbsoluteError = rows.Count == 0
            ? 0m
            : Math.Round(rows.Average(x => x.AbsoluteError), 2);

        decimal avgPercentageError = rows.Count == 0
            ? 0m
            : Math.Round(rows.Average(x => x.PercentageError), 2);

        ModelPerformanceCategoryResponse[] categories = rows
            .GroupBy(x => x.CategoryKey)
            .Select(g => new ModelPerformanceCategoryResponse
            {
                CategoryKey = g.Key,
                SoldListings = _dbContext.Listings.Count(x => x.CategoryKey == g.Key && x.Status == ListingStatuses.Sold),
                SoldListingsWithPrediction = g.Count(),
                AverageAbsoluteError = Math.Round(g.Average(x => x.AbsoluteError), 2),
                AveragePercentageError = Math.Round(g.Average(x => x.PercentageError), 2)
            })
            .OrderBy(x => x.CategoryKey)
            .ToArray();

        return new ModelPerformanceOverviewResponse
        {
            TotalSoldListings = totalSoldListings,
            SoldListingsWithPrediction = soldListingsWithPrediction,
            AverageAbsoluteError = avgAbsoluteError,
            AveragePercentageError = avgPercentageError,
            Categories = categories
        };
    }

    /// <inheritdoc />
    public async Task<ModelPerformanceCategoryResponse?> GetCategoryAsync(string categoryKey, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(categoryKey);

        List<PerformanceRow> rows = await QueryPerformanceRows()
            .Where(x => x.CategoryKey == categoryKey)
            .ToListAsync(cancellationToken);

        if (rows.Count == 0)
        {
            bool hasSoldListings = await _dbContext.Listings
                .AsNoTracking()
                .AnyAsync(x => x.CategoryKey == categoryKey && x.Status == ListingStatuses.Sold, cancellationToken);

            if (!hasSoldListings)
            {
                return null;
            }
        }

        int soldListings = await _dbContext.Listings
            .AsNoTracking()
            .CountAsync(x => x.CategoryKey == categoryKey && x.Status == ListingStatuses.Sold, cancellationToken);

        return new ModelPerformanceCategoryResponse
        {
            CategoryKey = categoryKey,
            SoldListings = soldListings,
            SoldListingsWithPrediction = rows.Count,
            AverageAbsoluteError = rows.Count == 0 ? 0m : Math.Round(rows.Average(x => x.AbsoluteError), 2),
            AveragePercentageError = rows.Count == 0 ? 0m : Math.Round(rows.Average(x => x.PercentageError), 2)
        };
    }

    /// <inheritdoc />
    public async Task<IReadOnlyCollection<ModelPerformanceListingResponse>> GetRecentListingsAsync(CancellationToken cancellationToken = default)
    {
        ModelPerformanceListingResponse[] rows = await QueryPerformanceRows()
            .OrderByDescending(x => x.SoldAtUtc)
            .Take(100)
            .Select(x => new ModelPerformanceListingResponse
            {
                ListingId = x.ListingId,
                CategoryKey = x.CategoryKey,
                Title = x.Title,
                AskingPrice = x.AskingPrice,
                PredictedPrice = x.PredictedPrice,
                SoldPrice = x.SoldPrice,
                AbsoluteError = x.AbsoluteError,
                PercentageError = x.PercentageError,
                SoldAtUtc = x.SoldAtUtc
            })
            .ToArrayAsync(cancellationToken);

        return rows;
    }

    private IQueryable<PerformanceRow> QueryPerformanceRows()
    {
        return _dbContext.Listings
            .AsNoTracking()
            .Where(x => x.Status == ListingStatuses.Sold && x.PredictedPrice.HasValue && x.SoldPrice.HasValue && x.SoldAtUtc.HasValue && x.SoldPrice.Value > 0)
            .Select(x => new PerformanceRow
            {
                ListingId = x.Id,
                CategoryKey = x.CategoryKey,
                Title = x.Title,
                AskingPrice = x.AskingPrice,
                PredictedPrice = x.PredictedPrice!.Value,
                SoldPrice = x.SoldPrice!.Value,
                AbsoluteError = Math.Abs(x.PredictedPrice!.Value - x.SoldPrice!.Value),
                PercentageError = Math.Abs(x.PredictedPrice!.Value - x.SoldPrice!.Value) / x.SoldPrice!.Value * 100m,
                SoldAtUtc = x.SoldAtUtc!.Value
            });
    }

    private sealed class PerformanceRow
    {
        public Guid ListingId { get; init; }
        public required string CategoryKey { get; init; }
        public required string Title { get; init; }
        public decimal AskingPrice { get; init; }
        public decimal PredictedPrice { get; init; }
        public decimal SoldPrice { get; init; }
        public decimal AbsoluteError { get; init; }
        public decimal PercentageError { get; init; }
        public DateTime SoldAtUtc { get; init; }
    }
}