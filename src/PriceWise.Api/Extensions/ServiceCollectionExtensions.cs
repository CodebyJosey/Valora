using PriceWise.Application.Interfaces;
using PriceWise.Infrastructure.ML.Models;
using Microsoft.Extensions.ML;
using PriceWise.Infrastructure.ML.Prediction;
using Microsoft.ML;
using PriceWise.Api.ML;

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
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IHostEnvironment env)
    {

        services.AddSingleton(new MLContext(seed: 1));
        services.AddScoped<IPricePredictor, ApiPricePredictor>();

        string repoRoot = Directory.GetParent(env.ContentRootPath)!.Parent!.FullName;
        string modelPath = Path.Combine(repoRoot, "artifacts", "models", "laptop-price-model.zip");

        services.AddPredictionEnginePool<LaptopPriceTrainingRow, LaptopPricePrediction>()
            .FromFile(modelName: "LaptopPriceModel", filePath: modelPath, watchForChanges: true);

        return services;
    }
}