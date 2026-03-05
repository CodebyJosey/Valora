using PriceWise.Application.Interfaces;
using PriceWise.Infrastructure.ML.Models;
using Microsoft.Extensions.ML;
using PriceWise.Infrastructure.ML.Prediction;
using Microsoft.ML;

namespace PriceWise.Api.Extensions;

/// <summary>
/// Extension methods for registering PriceWise services.
/// Keeps Program.cs clean and organizes DI by layer.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Registers API related services (controllers, swagger, etc.).
    /// </summary>
    public static IServiceCollection AddApiServices(this IServiceCollection services)
    {
        services.AddControllers();

        // Swagger (via Swashbuckle)
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();

        return services;
    }

    /// <summary>
    /// Registers Application layer services.
    /// (Empty for now - later: use cases, validators, Result handling, etc.)
    /// </summary>
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        return services;
    }

    /// <summary>
    /// Registers Infrastructure layer services (ML.NET predictors, model loading, etc.).
    /// </summary>
    /// <param name="services">Service collection.</param>
    /// <param name="env">Host environment used to resolve repo-root paths.</param>
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, string modelPath)
    {
        services.AddSingleton(new MLContext(seed: 1));

        services.AddSingleton<ITrainedModelProvider>(sp =>
        {
            MLContext? ml = sp.GetRequiredService<MLContext>();
            return new FileTrainedModelProvider(ml, modelPath);
        });

        services.AddScoped<IPricePredictor, MlNetPricePredictor>();

        return services;
    }
}