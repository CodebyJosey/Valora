"use client";

import { useCurrentUser } from "@/hooks/use-current-user";

export function DashboardHome(): React.JSX.Element {
  const { user } = useCurrentUser();

  return (
    <main className="mx-auto w-full max-w-7xl px-6 py-10 lg:px-8">
      <section className="soft-panel rounded-[2.2rem] p-8">
        <p className="text-sm font-bold uppercase tracking-[0.22em] text-emerald-900/50">
          Dashboard
        </p>

        <h1 className="mt-3 text-4xl font-black tracking-tight text-emerald-950">
          Welkom{user ? `, ${user.displayName}` : ""}
        </h1>

        <p className="mt-4 max-w-2xl text-sm leading-7 text-emerald-950/65">
          Je auth-flow werkt nu. In de volgende stap koppelen we hier je eigen
          listings, create listing, predict price en publish acties aan.
        </p>

        <div className="mt-8 grid gap-4 md:grid-cols-3">
          <div className="rounded-[1.5rem] bg-white p-5 shadow-sm">
            <p className="text-sm font-semibold text-emerald-900/55">Naam</p>
            <p className="mt-2 text-lg font-black text-emerald-950">
              {user?.displayName ?? "-"}
            </p>
          </div>

          <div className="rounded-[1.5rem] bg-white p-5 shadow-sm">
            <p className="text-sm font-semibold text-emerald-900/55">E-mail</p>
            <p className="mt-2 text-lg font-black text-emerald-950">
              {user?.email ?? "-"}
            </p>
          </div>

          <div className="rounded-[1.5rem] bg-white p-5 shadow-sm">
            <p className="text-sm font-semibold text-emerald-900/55">Rollen</p>
            <p className="mt-2 text-lg font-black text-emerald-950">
              {user?.roles.join(", ") ?? "-"}
            </p>
          </div>
        </div>
      </section>
    </main>
  );
}