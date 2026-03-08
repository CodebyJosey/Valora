"use client";

import { Search, X } from "lucide-react";

interface ListingSearchProps {
  value: string;
  onChange: (value: string) => void;
}

export function ListingSearch({
  value,
  onChange,
}: ListingSearchProps): React.JSX.Element {
  return (
    <div className="relative">
      <Search className="absolute left-4 top-1/2 h-4 w-4 -translate-y-1/2 text-emerald-900/40" />

      <input
        type="text"
        placeholder="Zoek op titel of beschrijving..."
        value={value}
        onChange={(event) => onChange(event.target.value)}
        className="w-full rounded-full border border-emerald-200 bg-white px-11 py-3 text-sm font-medium text-emerald-950 shadow-sm outline-none transition placeholder:text-emerald-900/35 focus:border-emerald-400"
      />

      {value.length > 0 ? (
        <button
          type="button"
          onClick={() => onChange("")}
          className="absolute right-3 top-1/2 flex h-8 w-8 -translate-y-1/2 items-center justify-center rounded-full text-emerald-900/45 transition hover:bg-emerald-50 hover:text-emerald-900"
          aria-label="Zoekopdracht wissen"
        >
          <X className="h-4 w-4" />
        </button>
      ) : null}
    </div>
  );
}