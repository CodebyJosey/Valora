using PriceWise.Api.Extensions;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Services.AddApiServices();
builder.Services.AddApplicationServices();
string repoRoot = Directory.GetParent(builder.Environment.ContentRootPath)!.Parent!.FullName;
string modelPath = Path.Combine(repoRoot, "artifacts", "models", "laptop-price-model.zip");
builder.Services.AddInfrastructureServices(modelPath);

WebApplication app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();

public partial class Program { }