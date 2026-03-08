import { ListingCard } from "@/components/listings/listing-card";
import type { ListingResponse } from "@/types/listings";

interface ListingGridProps {
  listings: ListingResponse[];
}

export function ListingGrid({
  listings,
}: ListingGridProps): React.JSX.Element {
  return (
    <div className="grid gap-6 md:grid-cols-2 xl:grid-cols-3">
      {listings.map((listing) => (
        <ListingCard key={listing.id} listing={listing} />
      ))}
    </div>
  );
}