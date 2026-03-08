interface ListingResultsHeaderProps {
  totalCount: number;
  filteredCount: number;
  search: string;
  selectedCategory: string;
}

function formatCategory(categoryKey: string): string {
  return categoryKey
    .replaceAll("-", " ")
    .replaceAll("_", " ")
    .replace(/\b\w/g, (char) => char.toUpperCase());
}

export function ListingResultsHeader({
  totalCount,
  filteredCount,
  search,
  selectedCategory,
}: ListingResultsHeaderProps): React.JSX.Element {
  return (
    <div className="flex flex-col gap-3 md:flex-row md:items-end md:justify-between">
      <div>
        <p className="text-sm font-bold uppercase tracking-[0.22em] text-emerald-900/45">
          Marketplace
        </p>
        <h1 className="mt-2 text-4xl font-black tracking-tight text-emerald-950 md:text-5xl">
          Browse listings
        </h1>
        <p className="mt-3 max-w-2xl text-sm leading-7 text-emerald-950/62">
          Live gekoppeld aan je echte backend endpoint voor gepubliceerde listings.
        </p>
      </div>

      <div className="rounded-[1.5rem] border border-emerald-200 bg-white px-5 py-4 shadow-sm">
        <p className="text-sm font-semibold text-emerald-950">
          {filteredCount} van {totalCount} listing{totalCount === 1 ? "" : "s"}
        </p>

        {(search || selectedCategory) ? (
          <p className="mt-1 text-xs text-emerald-900/55">
            {search ? `Zoekterm: "${search}"` : null}
            {search && selectedCategory ? " · " : null}
            {selectedCategory ? `Categorie: ${formatCategory(selectedCategory)}` : null}
          </p>
        ) : (
          <p className="mt-1 text-xs text-emerald-900/55">
            Geen filters actief
          </p>
        )}
      </div>
    </div>
  );
}