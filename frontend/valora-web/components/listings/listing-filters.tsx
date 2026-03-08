"use client";

interface ListingFiltersProps {
  categories: string[];
  selectedCategory: string;
  onCategoryChange: (value: string) => void;
  sortBy: string;
  onSortChange: (value: string) => void;
}

function formatCategory(categoryKey: string): string {
  return categoryKey
    .replaceAll("-", " ")
    .replaceAll("_", " ")
    .replace(/\b\w/g, (char) => char.toUpperCase());
}

export function ListingFilters({
  categories,
  selectedCategory,
  onCategoryChange,
  sortBy,
  onSortChange,
}: ListingFiltersProps): React.JSX.Element {
  return (
    <div className="grid gap-4 lg:grid-cols-[1fr_220px]">
      <div className="flex flex-wrap gap-2">
        <button
          type="button"
          onClick={() => onCategoryChange("")}
          className={`rounded-full px-4 py-2 text-sm font-semibold transition ${
            selectedCategory === ""
              ? "bg-emerald-900 text-white"
              : "border border-emerald-200 bg-white text-emerald-950 hover:bg-emerald-50"
          }`}
        >
          Alle categorieën
        </button>

        {categories.map((category) => (
          <button
            key={category}
            type="button"
            onClick={() => onCategoryChange(category)}
            className={`rounded-full px-4 py-2 text-sm font-semibold transition ${
              selectedCategory === category
                ? "bg-emerald-900 text-white"
                : "border border-emerald-200 bg-white text-emerald-950 hover:bg-emerald-50"
            }`}
          >
            {formatCategory(category)}
          </button>
        ))}
      </div>

      <div>
        <select
          value={sortBy}
          onChange={(event) => onSortChange(event.target.value)}
          className="w-full rounded-full border border-emerald-200 bg-white px-4 py-3 text-sm font-semibold text-emerald-950 shadow-sm outline-none transition focus:border-emerald-400"
        >
          <option value="newest">Nieuwste eerst</option>
          <option value="oldest">Oudste eerst</option>
          <option value="price-asc">Prijs laag → hoog</option>
          <option value="price-desc">Prijs hoog → laag</option>
          <option value="prediction-desc">AI prijs hoog → laag</option>
          <option value="title-asc">Titel A → Z</option>
        </select>
      </div>
    </div>
  );
}