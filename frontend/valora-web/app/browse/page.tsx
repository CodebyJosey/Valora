"use client";

import { useMemo, useState } from "react";
import { useQuery } from "@tanstack/react-query";
import { AlertTriangle, RefreshCcw, SlidersHorizontal } from "lucide-react";
import { Footer } from "@/components/layout/footer";
import { Navbar } from "@/components/layout/navbar";
import { ListingFilters } from "@/components/listings/listing-filters";
import { ListingGrid } from "@/components/listings/listing-grid";
import { ListingGridSkeleton } from "@/components/listings/listing-grid-skeleton";
import { ListingResultsHeader } from "@/components/listings/listing-results-header";
import { ListingSearch } from "@/components/listings/listing-search";
import { listingsApi } from "@/lib/api/listings-api";
import type { ListingResponse } from "@/types/listings";

type SortOption =
  | "newest"
  | "oldest"
  | "price-asc"
  | "price-desc"
  | "prediction-desc"
  | "title-asc";

function sortListings(listings: ListingResponse[], sortBy: SortOption): ListingResponse[] {
  const sorted = [...listings];

  switch (sortBy) {
    case "oldest":
      sorted.sort(
        (left, right) =>
          new Date(left.createdAtUtc).getTime() - new Date(right.createdAtUtc).getTime(),
      );
      return sorted;

    case "price-asc":
      sorted.sort((left, right) => left.askingPrice - right.askingPrice);
      return sorted;

    case "price-desc":
      sorted.sort((left, right) => right.askingPrice - left.askingPrice);
      return sorted;

    case "prediction-desc":
      sorted.sort(
        (left, right) => (right.predictedPrice ?? -1) - (left.predictedPrice ?? -1),
      );
      return sorted;

    case "title-asc":
      sorted.sort((left, right) => left.title.localeCompare(right.title, "nl-NL"));
      return sorted;

    case "newest":
    default:
      sorted.sort(
        (left, right) =>
          new Date(right.createdAtUtc).getTime() - new Date(left.createdAtUtc).getTime(),
      );
      return sorted;
  }
}

export default function BrowsePage(): React.JSX.Element {
  const [search, setSearch] = useState("");
  const [selectedCategory, setSelectedCategory] = useState("");
  const [sortBy, setSortBy] = useState<SortOption>("newest");

  const listingsQuery = useQuery({
    queryKey: ["published-listings"],
    queryFn: ({ signal }) => listingsApi.getPublished(signal),
  });

  const categories = useMemo(() => {
    if (!listingsQuery.data) {
      return [];
    }

    return [...new Set(listingsQuery.data.map((listing) => listing.categoryKey))]
      .filter((value) => value.trim().length > 0)
      .sort((left, right) => left.localeCompare(right, "nl-NL"));
  }, [listingsQuery.data]);

  const filteredListings = useMemo(() => {
    const source = listingsQuery.data ?? [];
    const normalizedSearch = search.trim().toLowerCase();

    const filtered = source.filter((listing) => {
      const matchesSearch =
        normalizedSearch.length === 0 ||
        listing.title.toLowerCase().includes(normalizedSearch) ||
        listing.description.toLowerCase().includes(normalizedSearch);

      const matchesCategory =
        selectedCategory.length === 0 || listing.categoryKey === selectedCategory;

      return matchesSearch && matchesCategory;
    });

    return sortListings(filtered, sortBy);
  }, [listingsQuery.data, search, selectedCategory, sortBy]);

  return (
    <>
      <Navbar />

      <main className="mx-auto w-full max-w-7xl px-6 py-10 lg:px-8 lg:py-14">
        <section className="soft-panel rounded-[2.2rem] p-8">
          <ListingResultsHeader
            totalCount={listingsQuery.data?.length ?? 0}
            filteredCount={filteredListings.length}
            search={search}
            selectedCategory={selectedCategory}
          />

          <div className="mt-8 grid gap-4">
            <ListingSearch value={search} onChange={setSearch} />

            <div className="rounded-[1.75rem] bg-emerald-50/70 p-4">
              <div className="mb-4 flex items-center gap-2 text-sm font-bold text-emerald-950">
                <SlidersHorizontal className="h-4 w-4" />
                Filters & sortering
              </div>

              <ListingFilters
                categories={categories}
                selectedCategory={selectedCategory}
                onCategoryChange={setSelectedCategory}
                sortBy={sortBy}
                onSortChange={(value) => setSortBy(value as SortOption)}
              />
            </div>
          </div>
        </section>

        <section className="mt-10">
          {listingsQuery.isLoading ? <ListingGridSkeleton /> : null}

          {listingsQuery.isError ? (
            <div className="soft-panel rounded-[2rem] p-8">
              <div className="flex items-start gap-4">
                <div className="flex h-12 w-12 items-center justify-center rounded-[1.2rem] bg-red-50">
                  <AlertTriangle className="h-5 w-5 text-red-600" />
                </div>

                <div className="flex-1">
                  <h2 className="text-xl font-black text-emerald-950">
                    Listings konden niet worden geladen
                  </h2>
                  <p className="mt-2 text-sm leading-7 text-emerald-950/65">
                    Controleer of je backend draait en of CORS en de API-base-url goed staan.
                  </p>

                  <button
                    type="button"
                    onClick={() => void listingsQuery.refetch()}
                    className="mt-5 inline-flex items-center gap-2 rounded-full bg-emerald-900 px-4 py-2 text-sm font-semibold text-white transition hover:bg-emerald-800"
                  >
                    <RefreshCcw className="h-4 w-4" />
                    Opnieuw proberen
                  </button>
                </div>
              </div>
            </div>
          ) : null}

          {!listingsQuery.isLoading &&
          !listingsQuery.isError &&
          filteredListings.length === 0 ? (
            <div className="soft-panel rounded-[2rem] p-8">
              <h2 className="text-2xl font-black text-emerald-950">
                Geen listings gevonden
              </h2>
              <p className="mt-3 max-w-2xl text-sm leading-7 text-emerald-950/65">
                Er zijn nu geen resultaten die passen bij je zoekopdracht of filters.
                Probeer een andere zoekterm of kies een andere categorie.
              </p>

              <div className="mt-6 flex flex-wrap gap-3">
                <button
                  type="button"
                  onClick={() => setSearch("")}
                  className="rounded-full border border-emerald-200 bg-white px-4 py-2 text-sm font-semibold text-emerald-950 transition hover:bg-emerald-50"
                >
                  Zoekopdracht wissen
                </button>

                <button
                  type="button"
                  onClick={() => {
                    setSelectedCategory("");
                    setSortBy("newest");
                  }}
                  className="rounded-full border border-emerald-200 bg-white px-4 py-2 text-sm font-semibold text-emerald-950 transition hover:bg-emerald-50"
                >
                  Filters resetten
                </button>
              </div>
            </div>
          ) : null}

          {!listingsQuery.isLoading &&
          !listingsQuery.isError &&
          filteredListings.length > 0 ? (
            <ListingGrid listings={filteredListings} />
          ) : null}
        </section>
      </main>

      <Footer />
    </>
  );
}