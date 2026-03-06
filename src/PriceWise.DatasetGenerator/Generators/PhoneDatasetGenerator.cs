using System.Globalization;
using System.Text;
using PriceWise.DatasetGenerator.Models;
using PriceWise.DatasetGenerator.Pricing;

namespace PriceWise.DatasetGenerator.Generators;

/// <summary>
/// Generates synthetic phone datasets for ML training.
/// </summary>
public sealed class PhoneDatasetGenerator
{
    private static readonly string[] Brands =
    {
        "Apple",
        "Samsung",
        "Google",
        "OnePlus",
        "Xiaomi",
        "Nothing",
        "Motorola"
    };

    private static readonly string[] Conditions =
    {
        "New",
        "Refurbished",
        "UsedGood",
        "UsedFair"
    };

    /// <summary>
    /// Generates a collection of phone rows.
    /// </summary>
    public IReadOnlyList<PhoneDatasetRow> Generate(int count, int seed = 42)
    {
        var random = new Random(seed);
        var rows = new List<PhoneDatasetRow>(count);

        for (int i = 0; i < count; i++)
        {
            rows.Add(GenerateOne(random));
        }

        return rows;
    }

    /// <summary>
    /// Writes the generated dataset to a CSV file.
    /// </summary>
    public void WriteCsv(string outputPath, IEnumerable<PhoneDatasetRow> rows)
    {
        string? directory = Path.GetDirectoryName(outputPath);

        if (!string.IsNullOrWhiteSpace(directory))
        {
            Directory.CreateDirectory(directory);
        }

        var sb = new StringBuilder();
        sb.AppendLine("Brand,ModelFamily,StorageGb,RamGb,BatteryHealth,Condition,ReleaseYear,Price");

        foreach (PhoneDatasetRow row in rows)
        {
            sb.Append(row.Brand).Append(',')
              .Append(row.ModelFamily).Append(',')
              .Append(row.StorageGb).Append(',')
              .Append(row.RamGb).Append(',')
              .Append(row.BatteryHealth).Append(',')
              .Append(row.Condition).Append(',')
              .Append(row.ReleaseYear).Append(',')
              .Append(row.Price.ToString(CultureInfo.InvariantCulture))
              .AppendLine();
        }

        File.WriteAllText(outputPath, sb.ToString(), Encoding.UTF8);
    }

    private static PhoneDatasetRow GenerateOne(Random random)
    {
        string brand = PickBrand(random);
        string modelFamily = PickModelFamily(brand, random);
        int storageGb = PickStorage(brand, modelFamily, random);
        int ramGb = PickRam(brand, modelFamily, random);
        string condition = PickCondition(random);
        int releaseYear = PickReleaseYear(modelFamily, random);
        int batteryHealth = PickBatteryHealth(condition, releaseYear, random);

        decimal price = PhonePricingRules.CalculatePrice(
            brand,
            modelFamily,
            storageGb,
            ramGb,
            batteryHealth,
            condition,
            releaseYear,
            random);

        return new PhoneDatasetRow
        {
            Brand = brand,
            ModelFamily = modelFamily,
            StorageGb = storageGb,
            RamGb = ramGb,
            BatteryHealth = batteryHealth,
            Condition = condition,
            ReleaseYear = releaseYear,
            Price = price
        };
    }

    private static string PickBrand(Random random)
        => Brands[random.Next(Brands.Length)];

    private static string PickModelFamily(string brand, Random random)
    {
        return brand switch
        {
            "Apple" => PickFrom(random, "iPhone11", "iPhone12", "iPhone13", "iPhone14", "iPhone15"),
            "Samsung" => PickFrom(random, "GalaxyS21", "GalaxyS22", "GalaxyS23", "GalaxyS24"),
            "Google" => PickFrom(random, "Pixel6", "Pixel7", "Pixel8", "Pixel9"),
            "OnePlus" => PickFrom(random, "OnePlus10", "OnePlus11", "OnePlus12"),
            "Xiaomi" => PickFrom(random, "Xiaomi13", "Xiaomi14"),
            "Nothing" => PickFrom(random, "NothingPhone1", "NothingPhone2"),
            "Motorola" => PickFrom(random, "MotoEdge40", "MotoEdge50"),
            _ => "UnknownPhone"
        };
    }

    private static int PickStorage(string brand, string modelFamily, Random random)
    {
        if (brand == "Apple")
        {
            return PickFrom(random, 128, 256, 512);
        }

        if (modelFamily.StartsWith("GalaxyS24", StringComparison.OrdinalIgnoreCase) ||
            modelFamily.StartsWith("Pixel9", StringComparison.OrdinalIgnoreCase) ||
            modelFamily.StartsWith("OnePlus12", StringComparison.OrdinalIgnoreCase))
        {
            return PickFrom(random, 128, 256, 512);
        }

        return PickFrom(random, 128, 256);
    }

    private static int PickRam(string brand, string modelFamily, Random random)
    {
        if (brand == "Apple")
        {
            return modelFamily switch
            {
                "iPhone11" => 4,
                "iPhone12" => 4,
                "iPhone13" => 4,
                "iPhone14" => 6,
                "iPhone15" => 6,
                _ => 6
            };
        }

        if (brand == "Samsung")
        {
            return PickFrom(random, 8, 12);
        }

        if (brand == "Google")
        {
            return PickFrom(random, 8, 12);
        }

        if (brand == "OnePlus")
        {
            return PickFrom(random, 8, 12, 16);
        }

        if (brand == "Xiaomi")
        {
            return PickFrom(random, 8, 12);
        }

        return PickFrom(random, 6, 8, 12);
    }

    private static string PickCondition(Random random)
    {
        return Conditions[random.Next(Conditions.Length)];
    }

    private static int PickReleaseYear(string modelFamily, Random random)
    {
        int year = modelFamily switch
        {
            "iPhone11" => 2019,
            "iPhone12" => 2020,
            "iPhone13" => 2021,
            "iPhone14" => 2022,
            "iPhone15" => 2023,

            "GalaxyS21" => 2021,
            "GalaxyS22" => 2022,
            "GalaxyS23" => 2023,
            "GalaxyS24" => 2024,

            "Pixel6" => 2021,
            "Pixel7" => 2022,
            "Pixel8" => 2023,
            "Pixel9" => 2024,

            "OnePlus10" => 2022,
            "OnePlus11" => 2023,
            "OnePlus12" => 2024,

            "Xiaomi13" => 2023,
            "Xiaomi14" => 2024,

            "NothingPhone1" => 2022,
            "NothingPhone2" => 2023,

            "MotoEdge40" => 2023,
            "MotoEdge50" => 2024,
            _ => DateTime.UtcNow.Year - 1
        };

        // Tiny variance for realism if you want near-launch / market timing effects
        return year;
    }

    private static int PickBatteryHealth(string condition, int releaseYear, Random random)
    {
        if (condition == "New")
        {
            return random.Next(98, 101);
        }

        int age = DateTime.UtcNow.Year - releaseYear;

        return condition switch
        {
            "Refurbished" => Math.Clamp(95 - age * 2 + random.Next(-2, 3), 85, 100),
            "UsedGood" => Math.Clamp(90 - age * 4 + random.Next(-4, 5), 75, 96),
            "UsedFair" => Math.Clamp(84 - age * 5 + random.Next(-5, 6), 65, 90),
            _ => 90
        };
    }

    private static string PickFrom(Random random, params string[] options)
        => options[random.Next(options.Length)];

    private static int PickFrom(Random random, params int[] options)
        => options[random.Next(options.Length)];
}