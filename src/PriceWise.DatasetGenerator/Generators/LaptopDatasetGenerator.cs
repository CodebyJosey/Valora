using System.Globalization;
using System.Text;
using PriceWise.DatasetGenerator.Models;
using PriceWise.DatasetGenerator.Pricing;

namespace PriceWise.DatasetGenerator.Generators;

/// <summary>
/// Generates synthetic laptop datasets for ML training.
/// </summary>
public sealed class LaptopDatasetGenerator
{
    private static readonly string[] Brands =
    {
        "Dell", "HP", "Lenovo", "Acer", "Asus", "MSI", "Apple"
    };

    private static readonly string[] IntelCpus =
    {
        "i3", "i5", "i7", "i9"
    };

    private static readonly string[] AppleCpus =
    {
        "M1", "M2", "M3"
    };

    private static readonly string[] Conditions =
    {
        "New", "Refurbished", "UsedGood", "UsedFair"
    };

    private static readonly string[] Segments =
    {
        "Budget", "Mainstream", "Gaming", "Ultrabook", "Workstation"
    };

    public IReadOnlyList<LaptopDatasetRow> Generate(int count, int seed = 42)
    {
        Random? random = new Random(seed);
        List<LaptopDatasetRow>? rows = new List<LaptopDatasetRow>(count);

        for (int i = 0; i < count; i++)
        {
            rows.Add(GenerateOne(random));
        }

        return rows;
    }

    public void WriteCsv(string outputPath, IEnumerable<LaptopDatasetRow> rows)
    {
        string? directory = Path.GetDirectoryName(outputPath);

        if (!string.IsNullOrWhiteSpace(directory))
        {
            Directory.CreateDirectory(directory);
        }

        StringBuilder? sb = new StringBuilder();
        sb.AppendLine("Brand,Cpu,RamGb,StorageGb,Gpu,ScreenSizeInch,RefreshRate,ReleaseYear,Condition,Segment,Price");

        foreach (LaptopDatasetRow row in rows)
        {
            sb.Append(row.Brand).Append(',')
              .Append(row.Cpu).Append(',')
              .Append(row.RamGb).Append(',')
              .Append(row.StorageGb).Append(',')
              .Append(row.Gpu).Append(',')
              .Append(row.ScreenSizeInch.ToString(CultureInfo.InvariantCulture)).Append(',')
              .Append(row.RefreshRate).Append(',')
              .Append(row.ReleaseYear).Append(',')
              .Append(row.Condition).Append(',')
              .Append(row.Segment).Append(',')
              .Append(row.Price.ToString(CultureInfo.InvariantCulture))
              .AppendLine();
        }

        File.WriteAllText(outputPath, sb.ToString(), Encoding.UTF8);
    }

    private static LaptopDatasetRow GenerateOne(Random random)
    {
        string brand = PickBrand(random);
        string segment = PickSegment(brand, random);
        string cpu = PickCpu(brand, segment, random);
        int ramGb = PickRam(brand, cpu, segment, random);
        int storageGb = PickStorage(brand, cpu, ramGb, segment, random);
        string gpu = PickGpu(brand, cpu, ramGb, segment, random);
        float screenSize = PickScreenSize(segment, brand, random);
        int refreshRate = PickRefreshRate(segment, gpu, random);
        int releaseYear = PickReleaseYear(conditionBiasNew: segment is "Ultrabook" or "Workstation", random);
        string condition = PickCondition(segment, random);

        decimal price = LaptopPricingRules.CalculatePrice(
            brand,
            cpu,
            ramGb,
            storageGb,
            gpu,
            screenSize,
            refreshRate,
            releaseYear,
            condition,
            segment,
            random);

        return new LaptopDatasetRow
        {
            Brand = brand,
            Cpu = cpu,
            RamGb = ramGb,
            StorageGb = storageGb,
            Gpu = gpu,
            ScreenSizeInch = screenSize,
            RefreshRate = refreshRate,
            ReleaseYear = releaseYear,
            Condition = condition,
            Segment = segment,
            Price = price
        };
    }

    private static string PickBrand(Random random)
        => Brands[random.Next(Brands.Length)];

    private static string PickSegment(string brand, Random random)
    {
        if (brand == "Apple")
        {
            string[] appleSegments = { "Mainstream", "Ultrabook", "Workstation" };
            return appleSegments[random.Next(appleSegments.Length)];
        }

        return Segments[random.Next(Segments.Length)];
    }

    private static string PickCpu(string brand, string segment, Random random)
    {
        if (brand == "Apple")
        {
            if (segment == "Workstation")
            {
                string[] highApple = { "M2", "M3" };
                return highApple[random.Next(highApple.Length)];
            }

            return AppleCpus[random.Next(AppleCpus.Length)];
        }

        if (segment == "Budget")
        {
            string[] low = { "i3", "i5" };
            return low[random.Next(low.Length)];
        }

        if (segment == "Gaming" || segment == "Workstation")
        {
            string[] high = { "i5", "i7", "i9" };
            return high[random.Next(high.Length)];
        }

        return IntelCpus[random.Next(IntelCpus.Length)];
    }

    private static int PickRam(string brand, string cpu, string segment, Random random)
    {
        if (brand == "Apple")
        {
            int[] appleRam = segment switch
            {
                "Ultrabook" => new[] { 8, 16, 24 },
                "Workstation" => new[] { 16, 24, 32 },
                _ => new[] { 8, 16, 24 }
            };

            return appleRam[random.Next(appleRam.Length)];
        }

        int[] options = segment switch
        {
            "Budget" => new[] { 8, 16 },
            "Mainstream" => new[] { 8, 16, 32 },
            "Gaming" => new[] { 16, 32, 64 },
            "Ultrabook" => new[] { 8, 16, 32 },
            "Workstation" => new[] { 16, 32, 64 },
            _ => new[] { 8, 16 }
        };

        if (cpu == "i3")
        {
            options = new[] { 8, 16 };
        }

        return options[random.Next(options.Length)];
    }

    private static int PickStorage(string brand, string cpu, int ramGb, string segment, Random random)
    {
        int[] options = segment switch
        {
            "Budget" => new[] { 256, 512 },
            "Mainstream" => new[] { 256, 512, 1024 },
            "Gaming" => new[] { 512, 1024, 2048 },
            "Ultrabook" => new[] { 256, 512, 1024 },
            "Workstation" => new[] { 512, 1024, 2048 },
            _ => new[] { 256, 512 }
        };

        if (brand == "Apple")
        {
            options = new[] { 256, 512, 1024, 2048 };
        }

        if (cpu == "i3")
        {
            options = new[] { 256, 512 };
        }

        if (ramGb >= 32 && !options.Contains(2048))
        {
            options = options.Append(2048).Distinct().ToArray();
        }

        return options[random.Next(options.Length)];
    }

    private static string PickGpu(string brand, string cpu, int ramGb, string segment, Random random)
    {
        if (brand == "Apple")
        {
            return "Integrated";
        }

        return segment switch
        {
            "Budget" => random.Next(100) < 80 ? "Integrated" : "RTX2050",
            "Mainstream" => PickFrom(random, "Integrated", "RTX2050", "RTX3050"),
            "Gaming" => PickFrom(random, "RTX3050", "RTX3060", "RTX4050", "RTX4060", "RTX4070", "RTX4080"),
            "Ultrabook" => random.Next(100) < 90 ? "Integrated" : "RTX2050",
            "Workstation" => PickFrom(random, "RTX3060", "RTX4050", "RTX4060", "RTX4070", "RTX4080"),
            _ => "Integrated"
        };
    }

    private static float PickScreenSize(string segment, string brand, Random random)
    {
        float[] options = segment switch
        {
            "Budget" => new[] { 14.0f, 15.6f },
            "Mainstream" => new[] { 14.0f, 15.6f, 16.0f },
            "Gaming" => new[] { 15.6f, 16.0f, 17.3f },
            "Ultrabook" => new[] { 13.3f, 14.0f },
            "Workstation" => new[] { 15.6f, 16.0f, 17.3f },
            _ => new[] { 15.6f }
        };

        if (brand == "Apple")
        {
            options = new[] { 13.6f, 14.2f, 16.2f };
        }

        return options[random.Next(options.Length)];
    }

    private static int PickRefreshRate(string segment, string gpu, Random random)
    {
        if (gpu == "Integrated")
        {
            return PickFrom(random, 60, 60, 60, 90, 120);
        }

        return segment switch
        {
            "Gaming" => PickFrom(random, 120, 144, 165, 240),
            "Workstation" => PickFrom(random, 60, 120, 144),
            _ => PickFrom(random, 60, 90, 120, 144)
        };
    }

    private static int PickReleaseYear(bool conditionBiasNew, Random random)
    {
        int currentYear = DateTime.UtcNow.Year;

        int[] years = conditionBiasNew
            ? new[] { currentYear, currentYear - 1, currentYear - 2 }
            : new[] { currentYear, currentYear - 1, currentYear - 2, currentYear - 3, currentYear - 4 };

        return years[random.Next(years.Length)];
    }

    private static string PickCondition(string segment, Random random)
    {
        string[] options = segment switch
        {
            "Budget" => new[] { "Refurbished", "UsedGood", "UsedFair", "New" },
            "Mainstream" => new[] { "New", "Refurbished", "UsedGood" },
            "Gaming" => new[] { "New", "UsedGood", "Refurbished" },
            "Ultrabook" => new[] { "New", "Refurbished", "UsedGood" },
            "Workstation" => new[] { "New", "Refurbished", "UsedGood" },
            _ => Conditions
        };

        return options[random.Next(options.Length)];
    }

    private static string PickFrom(Random random, params string[] options)
        => options[random.Next(options.Length)];

    private static int PickFrom(Random random, params int[] options)
        => options[random.Next(options.Length)];
}