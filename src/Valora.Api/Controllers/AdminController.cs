using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Valora.Application.Abstractions.Admin;
using Valora.Application.Contracts.Admin;
using Valora.Domain.Constants;
using Valora.Infrastructure.Persistence;
using Valora.Infrastructure.Persistence.Identity;

namespace Valora.Api.Controllers;

/// <summary>
/// Administrative endpoints for platform insights.
/// </summary>
[ApiController]
[Authorize(Roles = ApplicationRoles.Admin)]
[Route("api/admin")]
public sealed class AdminController : ControllerBase
{
    private readonly ValoraDbContext _dbContext;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IModelPerformanceService _modelPerformanceService;

    /// <summary>
    /// Initializes a new instance of the <see cref="AdminController"/> class.
    /// </summary>
    public AdminController(
        ValoraDbContext dbContext,
        UserManager<ApplicationUser> userManager,
        IModelPerformanceService modelPerformanceService)
    {
        _dbContext = dbContext;
        _userManager = userManager;
        _modelPerformanceService = modelPerformanceService;
    }

    /// <summary>
    /// Gets a combined admin dashboard overview.
    /// </summary>
    [HttpGet("dashboard")]
    [ProducesResponseType(typeof(AdminDashboardResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetDashboard(CancellationToken cancellationToken)
    {
        DateTime sinceUtc = DateTime.UtcNow.AddHours(-24);

        int totalUsers = await _dbContext.Users.CountAsync(cancellationToken);
        int totalListings = await _dbContext.Listings.CountAsync(cancellationToken);
        int publishedListings = await _dbContext.Listings.CountAsync(x => x.Status == ListingStatuses.Published, cancellationToken);
        int soldListings = await _dbContext.Listings.CountAsync(x => x.Status == ListingStatuses.Sold, cancellationToken);
        int totalPredictions = await _dbContext.PredictionRecords.CountAsync(cancellationToken);
        int totalTrainingRuns = await _dbContext.TrainingRuns.CountAsync(cancellationToken);
        int totalAuditLogs = await _dbContext.AuditLogs.CountAsync(cancellationToken);
        int listingsLast24Hours = await _dbContext.Listings.CountAsync(x => x.CreatedAtUtc >= sinceUtc, cancellationToken);
        int predictionsLast24Hours = await _dbContext.PredictionRecords.CountAsync(x => x.CreatedAtUtc >= sinceUtc, cancellationToken);

        AdminMetricItemResponse[] listingsByCategory = await _dbContext.Listings
            .AsNoTracking()
            .GroupBy(x => x.CategoryKey)
            .Select(g => new AdminMetricItemResponse
            {
                Name = g.Key,
                Value = g.Count()
            })
            .OrderByDescending(x => x.Value)
            .ToArrayAsync(cancellationToken);

        AdminMetricItemResponse[] predictionsByCategory = await _dbContext.PredictionRecords
            .AsNoTracking()
            .GroupBy(x => x.CategoryKey)
            .Select(g => new AdminMetricItemResponse
            {
                Name = g.Key,
                Value = g.Count()
            })
            .OrderByDescending(x => x.Value)
            .ToArrayAsync(cancellationToken);

        AdminMetricItemResponse[] trainingRunsByCategory = await _dbContext.TrainingRuns
            .AsNoTracking()
            .GroupBy(x => x.CategoryKey)
            .Select(g => new AdminMetricItemResponse
            {
                Name = g.Key,
                Value = g.Count()
            })
            .OrderByDescending(x => x.Value)
            .ToArrayAsync(cancellationToken);

        AdminRecentAuditLogResponse[] recentAuditLogs = await _dbContext.AuditLogs
            .AsNoTracking()
            .OrderByDescending(x => x.CreatedAtUtc)
            .Take(20)
            .Select(x => new AdminRecentAuditLogResponse
            {
                Id = x.Id,
                UserId = x.UserId,
                Action = x.Action,
                EntityType = x.EntityType,
                EntityId = x.EntityId,
                Details = x.Details,
                CreatedAtUtc = x.CreatedAtUtc
            })
            .ToArrayAsync(cancellationToken);

        AdminDashboardResponse response = new()
        {
            TotalUsers = totalUsers,
            TotalListings = totalListings,
            PublishedListings = publishedListings,
            SoldListings = soldListings,
            TotalPredictions = totalPredictions,
            TotalTrainingRuns = totalTrainingRuns,
            TotalAuditLogs = totalAuditLogs,
            ListingsLast24Hours = listingsLast24Hours,
            PredictionsLast24Hours = predictionsLast24Hours,
            ListingsByCategory = listingsByCategory,
            PredictionsByCategory = predictionsByCategory,
            TrainingRunsByCategory = trainingRunsByCategory,
            RecentAuditLogs = recentAuditLogs
        };

        return Ok(response);
    }

    /// <summary>
    /// Gets simple user statistics.
    /// </summary>
    [HttpGet("users/stats")]
    [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetUserStats(CancellationToken cancellationToken)
    {
        ApplicationUser[] users = await _dbContext.Users
            .AsNoTracking()
            .ToArrayAsync(cancellationToken);

        int totalUsers = users.Length;
        int activeUsers = users.Count(x => x.IsActive);
        int admins = 0;
        int sellers = 0;
        int buyers = 0;

        foreach (ApplicationUser user in users)
        {
            IList<string> roles = await _userManager.GetRolesAsync(user);

            if (roles.Contains(ApplicationRoles.Admin))
            {
                admins++;
            }

            if (roles.Contains(ApplicationRoles.Seller))
            {
                sellers++;
            }

            if (roles.Contains(ApplicationRoles.Buyer))
            {
                buyers++;
            }
        }

        return Ok(new
        {
            totalUsers,
            activeUsers,
            admins,
            sellers,
            buyers
        });
    }

    /// <summary>
    /// Gets listing statistics.
    /// </summary>
    [HttpGet("listings/stats")]
    [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetListingStats(CancellationToken cancellationToken)
    {
        int total = await _dbContext.Listings.CountAsync(cancellationToken);
        int draft = await _dbContext.Listings.CountAsync(x => x.Status == ListingStatuses.Draft, cancellationToken);
        int predicted = await _dbContext.Listings.CountAsync(x => x.Status == ListingStatuses.Predicted, cancellationToken);
        int published = await _dbContext.Listings.CountAsync(x => x.Status == ListingStatuses.Published, cancellationToken);
        int sold = await _dbContext.Listings.CountAsync(x => x.Status == ListingStatuses.Sold, cancellationToken);
        int archived = await _dbContext.Listings.CountAsync(x => x.Status == ListingStatuses.Archived, cancellationToken);

        var byCategory = await _dbContext.Listings
            .AsNoTracking()
            .GroupBy(x => x.CategoryKey)
            .Select(g => new
            {
                category = g.Key,
                count = g.Count()
            })
            .OrderByDescending(x => x.count)
            .ToArrayAsync(cancellationToken);

        return Ok(new
        {
            total,
            draft,
            predicted,
            published,
            sold,
            archived,
            byCategory
        });
    }

    /// <summary>
    /// Gets prediction statistics.
    /// </summary>
    [HttpGet("predictions/stats")]
    [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetPredictionStats(CancellationToken cancellationToken)
    {
        int total = await _dbContext.PredictionRecords.CountAsync(cancellationToken);

        var byCategory = await _dbContext.PredictionRecords
            .AsNoTracking()
            .GroupBy(x => x.CategoryKey)
            .Select(g => new
            {
                category = g.Key,
                count = g.Count()
            })
            .OrderByDescending(x => x.count)
            .ToArrayAsync(cancellationToken);

        var byModelVersion = await _dbContext.PredictionRecords
            .AsNoTracking()
            .GroupBy(x => new { x.CategoryKey, x.ModelVersion })
            .Select(g => new
            {
                category = g.Key.CategoryKey,
                modelVersion = g.Key.ModelVersion,
                count = g.Count()
            })
            .OrderByDescending(x => x.count)
            .ToArrayAsync(cancellationToken);

        return Ok(new
        {
            total,
            byCategory,
            byModelVersion
        });
    }

    /// <summary>
    /// Gets model training statistics.
    /// </summary>
    [HttpGet("models/stats")]
    [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetModelStats(CancellationToken cancellationToken)
    {
        int totalTrainingRuns = await _dbContext.TrainingRuns.CountAsync(cancellationToken);

        var latestRuns = await _dbContext.TrainingRuns
            .AsNoTracking()
            .OrderByDescending(x => x.CreatedAtUtc)
            .Take(20)
            .Select(x => new
            {
                x.Id,
                x.CategoryKey,
                x.Version,
                x.RSquared,
                x.RootMeanSquaredError,
                x.MeanAbsoluteError,
                x.MeanSquaredError,
                x.CreatedAtUtc
            })
            .ToArrayAsync(cancellationToken);

        var byCategory = await _dbContext.TrainingRuns
            .AsNoTracking()
            .GroupBy(x => x.CategoryKey)
            .Select(g => new
            {
                category = g.Key,
                count = g.Count(),
                latestVersion = g.Max(x => x.Version)
            })
            .OrderByDescending(x => x.count)
            .ToArrayAsync(cancellationToken);

        return Ok(new
        {
            totalTrainingRuns,
            byCategory,
            latestRuns
        });
    }

    /// <summary>
    /// Gets recent audit logs.
    /// </summary>
    [HttpGet("audit-logs")]
    [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAuditLogs(CancellationToken cancellationToken)
    {
        var logs = await _dbContext.AuditLogs
            .AsNoTracking()
            .OrderByDescending(x => x.CreatedAtUtc)
            .Take(100)
            .Select(x => new
            {
                x.Id,
                x.UserId,
                x.Action,
                x.EntityType,
                x.EntityId,
                x.Details,
                x.CreatedAtUtc
            })
            .ToArrayAsync(cancellationToken);

        return Ok(new
        {
            count = logs.Length,
            logs
        });
    }

    /// <summary>
    /// Gets the model performance overview.
    /// </summary>
    [HttpGet("model-performance/overview")]
    [ProducesResponseType(typeof(ModelPerformanceOverviewResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetModelPerformanceOverview(CancellationToken cancellationToken)
    {
        ModelPerformanceOverviewResponse response =
            await _modelPerformanceService.GetOverviewAsync(cancellationToken);

        return Ok(response);
    }

    /// <summary>
    /// Gets model performance for a category.
    /// </summary>
    [HttpGet("model-performance/{category}")]
    [ProducesResponseType(typeof(ModelPerformanceCategoryResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetModelPerformanceByCategory(
        [FromRoute] string category,
        CancellationToken cancellationToken)
    {
        ModelPerformanceCategoryResponse? response =
            await _modelPerformanceService.GetCategoryAsync(category, cancellationToken);

        if (response is null)
        {
            return NotFound(new ProblemDetails
            {
                Title = "Model performance not found",
                Detail = $"No sold listing performance data exists yet for category '{category}'.",
                Status = StatusCodes.Status404NotFound
            });
        }

        return Ok(response);
    }

    /// <summary>
    /// Gets recent listing-level model performance rows.
    /// </summary>
    [HttpGet("model-performance/listings")]
    [ProducesResponseType(typeof(IReadOnlyCollection<ModelPerformanceListingResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetModelPerformanceListings(CancellationToken cancellationToken)
    {
        IReadOnlyCollection<ModelPerformanceListingResponse> response =
            await _modelPerformanceService.GetRecentListingsAsync(cancellationToken);

        return Ok(response);
    }
}