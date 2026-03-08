export function ListingGridSkeleton(): React.JSX.Element {
  return (
    <div className="grid gap-6 md:grid-cols-2 xl:grid-cols-3">
      {Array.from({ length: 6 }).map((_, index) => (
        <div
          key={index}
          className="overflow-hidden rounded-[2rem] border border-emerald-950/8 bg-white/90 shadow-[0_18px_60px_rgba(16,24,40,0.06)]"
        >
          <div className="h-44 animate-pulse bg-emerald-100" />
          <div className="space-y-4 p-6">
            <div className="h-3 w-24 animate-pulse rounded-full bg-emerald-100" />
            <div className="h-7 w-2/3 animate-pulse rounded-full bg-emerald-100" />
            <div className="h-4 w-full animate-pulse rounded-full bg-emerald-100" />
            <div className="h-4 w-5/6 animate-pulse rounded-full bg-emerald-100" />
            <div className="grid grid-cols-2 gap-3">
              <div className="h-20 animate-pulse rounded-[1.4rem] bg-emerald-100" />
              <div className="h-20 animate-pulse rounded-[1.4rem] bg-lime-100" />
            </div>
            <div className="h-10 w-32 animate-pulse rounded-full bg-emerald-200" />
          </div>
        </div>
      ))}
    </div>
  );
}