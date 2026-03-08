import Link from "next/link";

export function Footer(): React.JSX.Element {
  return (
    <footer className="mt-20 border-t border-emerald-950/8 bg-white/60">
      <div className="mx-auto flex w-full max-w-7xl flex-col gap-6 px-6 py-10 text-sm text-emerald-950/60 lg:flex-row lg:items-center lg:justify-between lg:px-8">
        <div>
          <p className="font-bold text-emerald-950">Valora</p>
          <p className="mt-1">Marketplace pricing met een frisse AI-first UX.</p>
        </div>

        <div className="flex flex-wrap gap-5">
          <Link href="/" className="transition hover:text-emerald-950">
            Home
          </Link>
          <Link href="/browse" className="transition hover:text-emerald-950">
            Browse
          </Link>
          <Link href="/login" className="transition hover:text-emerald-950">
            Inloggen
          </Link>
          <Link href="/register" className="transition hover:text-emerald-950">
            Registreren
          </Link>
        </div>
      </div>
    </footer>
  );
}