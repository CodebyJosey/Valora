namespace PriceWise.DatasetGenerator.Pricing;

/// <summary>
/// Contains pricing rules for synthetic laptop dataset generation.
/// </summary>
public static class LaptopPricingRules
{
    public static decimal CalculatePrice(
        string brand,
        string cpu,
        int ramGb,
        int storageGb,
        string gpu,
        float screenSizeInch,
        int refreshRate,
        int releaseYear,
        string condition,
        string segment,
        Random random)
    {
        decimal price = 250m;

        price += GetBrandModifier(brand);
        price += GetCpuModifier(cpu);
        price += GetRamModifier(ramGb);
        price += GetStorageModifier(storageGb);
        price += GetGpuModifier(gpu);
        price += GetScreenSizeModifier(screenSizeInch);
        price += GetRefreshRateModifier(refreshRate);
        price += GetReleaseYearModifier(releaseYear);
        price += GetConditionModifier(condition);
        price += GetSegmentModifier(segment);

        price += GetCombinationBonus(brand, cpu, ramGb, storageGb, gpu, refreshRate, segment);

        // Market imperfections: percentage-based noise.
        decimal percentageNoise = (decimal)(random.NextDouble() * 0.24 - 0.12); // -12% to +12%
        price += price * percentageNoise;

        // Small flat market fluctuation.
        price += random.Next(-80, 121);

        // Rare anomaly: overpriced or underpriced listing
        int anomalyRoll = random.Next(100);
        if (anomalyRoll < 4)
        {
            price *= 0.82m;
        }
        else if (anomalyRoll > 95)
        {
            price *= 1.15m;
        }

        if (price < 250m)
        {
            price = 250m;
        }

        return decimal.Round(price, 2);
    }

    private static decimal GetBrandModifier(string brand) => brand switch
    {
        "Dell" => 100m,
        "HP" => 80m,
        "Lenovo" => 90m,
        "Acer" => 40m,
        "Asus" => 95m,
        "MSI" => 170m,
        "Apple" => 380m,
        _ => 0m
    };

    private static decimal GetCpuModifier(string cpu) => cpu switch
    {
        "i3" => 70m,
        "i5" => 170m,
        "i7" => 320m,
        "i9" => 520m,
        "M1" => 280m,
        "M2" => 430m,
        "M3" => 600m,
        _ => 0m
    };

    private static decimal GetRamModifier(int ramGb) => ramGb switch
    {
        8 => 70m,
        16 => 180m,
        24 => 260m,
        32 => 360m,
        64 => 700m,
        _ => ramGb * 10m
    };

    private static decimal GetStorageModifier(int storageGb) => storageGb switch
    {
        256 => 60m,
        512 => 140m,
        1024 => 280m,
        2048 => 520m,
        _ => storageGb * 0.2m
    };

    private static decimal GetGpuModifier(string gpu) => gpu switch
    {
        "Integrated" => 0m,
        "RTX2050" => 150m,
        "RTX3050" => 260m,
        "RTX3060" => 380m,
        "RTX4050" => 500m,
        "RTX4060" => 650m,
        "RTX4070" => 900m,
        "RTX4080" => 1250m,
        _ => 0m
    };

    private static decimal GetScreenSizeModifier(float screenSizeInch)
    {
        if (screenSizeInch <= 13.9f) return 80m;
        if (screenSizeInch <= 15.6f) return 40m;
        if (screenSizeInch <= 16.1f) return 60m;
        return 30m;
    }

    private static decimal GetRefreshRateModifier(int refreshRate) => refreshRate switch
    {
        60 => 0m,
        90 => 30m,
        120 => 70m,
        144 => 120m,
        165 => 160m,
        240 => 250m,
        _ => 0m
    };

    private static decimal GetReleaseYearModifier(int releaseYear)
    {
        int age = DateTime.UtcNow.Year - releaseYear;

        return age switch
        {
            <= 0 => 180m,
            1 => 120m,
            2 => 50m,
            3 => -30m,
            4 => -100m,
            _ => -180m
        };
    }

    private static decimal GetConditionModifier(string condition) => condition switch
    {
        "New" => 180m,
        "Refurbished" => -40m,
        "UsedGood" => -120m,
        "UsedFair" => -220m,
        _ => 0m
    };

    private static decimal GetSegmentModifier(string segment) => segment switch
    {
        "Budget" => -100m,
        "Mainstream" => 0m,
        "Gaming" => 220m,
        "Ultrabook" => 180m,
        "Workstation" => 320m,
        _ => 0m
    };

    private static decimal GetCombinationBonus(
        string brand,
        string cpu,
        int ramGb,
        int storageGb,
        string gpu,
        int refreshRate,
        string segment)
    {
        decimal bonus = 0m;

        if (segment == "Gaming" && gpu != "Integrated")
        {
            bonus += 140m;
        }

        if (segment == "Workstation" && ramGb >= 32 && storageGb >= 1024)
        {
            bonus += 220m;
        }

        if (segment == "Ultrabook" && gpu == "Integrated" && storageGb >= 512)
        {
            bonus += 100m;
        }

        if (brand == "Apple")
        {
            bonus += 120m;
        }

        if (brand == "MSI" && gpu != "Integrated")
        {
            bonus += 140m;
        }

        if ((cpu == "i9" || cpu == "M3") && ramGb >= 32)
        {
            bonus += 180m;
        }

        if (refreshRate >= 144 && gpu != "Integrated")
        {
            bonus += 80m;
        }

        return bonus;
    }
}