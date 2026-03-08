import { parseFeaturesJson } from "@/lib/listings/feature-parser";

interface FeatureBadgesProps {
  featuresJson: string;
  maxItems?: number;
}

export function FeatureBadges({
  featuresJson,
  maxItems = 6,
}: FeatureBadgesProps): React.JSX.Element | null {
  const features = parseFeaturesJson(featuresJson).slice(0, maxItems);

  if (features.length === 0) {
    return null;
  }

  return (
    <div className="flex flex-wrap gap-2">
      {features.map((feature) => (
        <div
          key={feature.key}
          className="rounded-full border border-emerald-200 bg-white px-3 py-1.5 text-xs font-medium text-emerald-950/75 shadow-sm"
        >
          <span className="font-bold text-emerald-950">{feature.label}:</span>{" "}
          <span>{feature.value}</span>
        </div>
      ))}
    </div>
  );
}