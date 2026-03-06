using PriceWise.DatasetGenerator.Generators;
using PriceWise.DatasetGenerator.Models;

const int defaultCount = 2500;
const int defaultSeed = 42;

string repoRoot = Directory.GetParent(AppContext.BaseDirectory)!
    .Parent!
    .Parent!
    .Parent!
    .Parent!
    .FullName;

if (args.Length == 0)
{
    Console.WriteLine("Usage:");
    Console.WriteLine("  dotnet run --project src/PriceWise.DatasetGenerator -- laptops [count] [seed]");
    Console.WriteLine("  dotnet run --project src/PriceWise.DatasetGenerator -- phones [count] [seed]");
    return;
}

string datasetType = args[0].Trim().ToLowerInvariant();

int count = defaultCount;
int seed = defaultSeed;

if (args.Length >= 2 && int.TryParse(args[1], out int parsedCount) && parsedCount > 0)
{
    count = parsedCount;
}

if (args.Length >= 3 && int.TryParse(args[2], out int parsedSeed))
{
    seed = parsedSeed;
}

switch (datasetType)
{
    case "laptops":
        {
            string outputPath = Path.Combine(repoRoot, "..", "data", "datasets", "laptops.csv");

            LaptopDatasetGenerator? generator = new LaptopDatasetGenerator();
            IReadOnlyList<LaptopDatasetRow> rows = generator.Generate(count, seed);
            generator.WriteCsv(outputPath, rows);

            Console.WriteLine($"Generated {rows.Count} laptop rows.");
            Console.WriteLine($"Output: {outputPath}");
            Console.WriteLine($"Seed: {seed}");
            break;
        }

    case "phones":
        {
            string outputPath = Path.Combine(repoRoot, "..", "data", "datasets", "phones.csv");

            PhoneDatasetGenerator? generator = new PhoneDatasetGenerator();
            IReadOnlyList<PhoneDatasetRow> rows = generator.Generate(count, seed);
            generator.WriteCsv(outputPath, rows);

            Console.WriteLine($"Generated {rows.Count} phone rows.");
            Console.WriteLine($"Output: {outputPath}");
            Console.WriteLine($"Seed: {seed}");
            break;
        }

    default:
        Console.WriteLine($"Unknown dataset type: '{datasetType}'");
        Console.WriteLine("Supported types: laptops, phones");
        break;
}