namespace PriceWise.DatasetGenerator.Pricing;

/// <summary>
/// Contains pricing rules for synthetic phone dataset generation.
/// </summary>
public static class PhonePricingRules
{
    /// <summary>
    /// Calculates a synthetic but realistic phone price.
    /// </summary>
    public static decimal CalculatePrice(
        string brand,
        string modelFamily,
        int storageGb,
        int ramGb,
        int batteryHealth,
        string condition,
        int releaseYear,
        Random random)
    {
        decimal price = 120m;

        price += GetBrandModifier(brand);
        price += GetModelFamilyModifier(modelFamily);
        price += GetStorageModifier(storageGb);
        price += GetRamModifier(ramGb);
        price += GetBatteryHealthModifier(batteryHealth);
        price += GetConditionModifier(condition);
        price += GetReleaseYearModifier(releaseYear);

        price += GetCombinationBonus(brand, modelFamily, storageGb, ramGb, batteryHealth, condition);

        // Percentage-based market noise
        decimal noisePercentage = (decimal)(random.NextDouble() * 0.20 - 0.10); // -10% to +10%
        price += price * noisePercentage;

        // Small flat market fluctuation
        price += random.Next(-40, 61);

        // Rare underpriced / overpriced listing
        int anomalyRoll = random.Next(100);
        if (anomalyRoll < 4)
        {
            price *= 0.84m;
        }
        else if (anomalyRoll > 95)
        {
            price *= 1.12m;
        }

        if (price < 75m)
        {
            price = 75m;
        }

        return decimal.Round(price, 2);
    }

    private static decimal GetBrandModifier(string brand) => brand switch
    {
        "Apple" => 260m,
        "Samsung" => 180m,
        "Google" => 140m,
        "OnePlus" => 120m,
        "Xiaomi" => 70m,
        "Nothing" => 90m,
        "Motorola" => 50m,
        _ => 0m
    };

    private static decimal GetModelFamilyModifier(string modelFamily) => modelFamily switch
    {
        "iPhone11" => 120m,
        "iPhone12" => 180m,
        "iPhone13" => 260m,
        "iPhone14" => 340m,
        "iPhone15" => 460m,

        "GalaxyS21" => 170m,
        "GalaxyS22" => 250m,
        "GalaxyS23" => 340m,
        "GalaxyS24" => 460m,

        "Pixel6" => 140m,
        "Pixel7" => 220m,
        "Pixel8" => 320m,
        "Pixel9" => 430m,

        "OnePlus10" => 180m,
        "OnePlus11" => 270m,
        "OnePlus12" => 380m,

        "Xiaomi13" => 180m,
        "Xiaomi14" => 300m,

        "NothingPhone1" => 160m,
        "NothingPhone2" => 260m,

        "MotoEdge40" => 180m,
        "MotoEdge50" => 280m,
        _ => 0m
    };

    private static decimal GetStorageModifier(int storageGb) => storageGb switch
    {
        64 => 0m,
        128 => 60m,
        256 => 140m,
        512 => 280m,
        1024 => 520m,
        _ => storageGb * 0.4m
    };

    private static decimal GetRamModifier(int ramGb) => ramGb switch
    {
        4 => 0m,
        6 => 30m,
        8 => 70m,
        12 => 130m,
        16 => 220m,
        _ => ramGb * 10m
    };

    private static decimal GetBatteryHealthModifier(int batteryHealth)
    {
        if (batteryHealth >= 98) return 110m;
        if (batteryHealth >= 95) return 80m;
        if (batteryHealth >= 90) return 40m;
        if (batteryHealth >= 85) return 0m;
        if (batteryHealth >= 80) return -50m;
        return -120m;
    }

    private static decimal GetConditionModifier(string condition) => condition switch
    {
        "New" => 180m,
        "Refurbished" => 20m,
        "UsedGood" => -80m,
        "UsedFair" => -160m,
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
            3 => -40m,
            4 => -120m,
            _ => -220m
        };
    }

    private static decimal GetCombinationBonus(
        string brand,
        string modelFamily,
        int storageGb,
        int ramGb,
        int batteryHealth,
        string condition)
    {
        decimal bonus = 0m;

        if (brand == "Apple")
        {
            bonus += 80m;
        }

        if ((modelFamily.StartsWith("iPhone15", StringComparison.OrdinalIgnoreCase) ||
             modelFamily.StartsWith("GalaxyS24", StringComparison.OrdinalIgnoreCase) ||
             modelFamily.StartsWith("Pixel9", StringComparison.OrdinalIgnoreCase)) &&
            storageGb >= 256)
        {
            bonus += 120m;
        }

        if (ramGb >= 12 && storageGb >= 256)
        {
            bonus += 70m;
        }

        if (batteryHealth <= 82 && condition != "New")
        {
            bonus -= 80m;
        }

        if (condition == "Refurbished" && batteryHealth >= 90)
        {
            bonus += 50m;
        }

        return bonus;
    }
}