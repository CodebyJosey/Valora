import Link from "next/link";
import {
  ArrowRight,
  BarChart3,
  Search,
  ShieldCheck,
  Sparkles,
  WandSparkles,
} from "lucide-react";
import { Footer } from "@/components/layout/footer";
import { Navbar } from "@/components/layout/navbar";

const features = [
  {
    title: "AI-prijsvoorspellingen",
    description:
      "Bepaal sneller een realistische vraagprijs op basis van listing-features en model-output.",
    icon: Sparkles,
  },
  {
    title: "Frisse browse-ervaring",
    description:
      "Een moderne marketplace flow met focus op overzicht, vertrouwen en zachte UX.",
    icon: Search,
  },
  {
    title: "Seller & admin tools",
    description:
      "Van listingbeheer tot performance monitoring, alles vanuit één productgevoel.",
    icon: BarChart3,
  },
  {
    title: "Veilige toegang",
    description:
      "Strakke auth-flow voor buyers, sellers en admins zonder rommelige frontend.",
    icon: ShieldCheck,
  },
];

export default function HomePage(): React.JSX.Element {
  return (
    <>
      <Navbar />

      <main className="bubble-shell overflow-hidden">
        <section className="relative">
          <div className="mx-auto grid w-full max-w-7xl gap-12 px-6 pb-20 pt-14 lg:grid-cols-[1.12fr_0.88fr] lg:px-8 lg:pb-28 lg:pt-20">
            <div className="relative z-10 max-w-3xl">
              <div className="inline-flex rounded-full border border-emerald-200 bg-white/85 px-4 py-2 text-sm font-semibold text-emerald-900 shadow-sm">
                Mint marketplace UI · AI pricing · modern product feel
              </div>

              <h1 className="mt-6 text-5xl font-black leading-tight tracking-tight text-emerald-950 md:text-7xl">
                Een frisse marketplace voor
                <span className="green-gradient-text"> slimme prijsinzichten</span>
              </h1>

              <p className="mt-6 max-w-2xl text-lg leading-8 text-emerald-950/68 md:text-xl">
                Valora helpt verkopers betere listings maken en koppelt je
                browse-ervaring direct aan echte marketplace data uit je backend.
              </p>

              <div className="mt-10 flex flex-col gap-4 sm:flex-row">
                <Link
                  href="/browse"
                  className="inline-flex items-center justify-center gap-2 rounded-full bg-emerald-900 px-6 py-3.5 text-sm font-bold text-white transition hover:scale-[1.02]"
                >
                  Browse listings
                  <ArrowRight className="h-4 w-4" />
                </Link>

                <Link
                  href="/register"
                  className="inline-flex items-center justify-center rounded-full border border-emerald-200 bg-white px-6 py-3.5 text-sm font-bold text-emerald-950 shadow-sm transition hover:bg-emerald-50"
                >
                  Start als seller
                </Link>
              </div>

              <div className="mt-10 flex flex-wrap gap-6 text-sm text-emerald-950/60">
                <div>
                  <span className="font-black text-emerald-950">Echte</span>{" "}
                  listing data
                </div>
                <div>
                  <span className="font-black text-emerald-950">Zachte</span>{" "}
                  bubble UI
                </div>
                <div>
                  <span className="font-black text-emerald-950">Schaalbare</span>{" "}
                  frontend basis
                </div>
              </div>
            </div>

            <div className="relative z-10">
              <div className="soft-panel rounded-[2.2rem] p-5">
                <div className="rounded-[1.8rem] bg-gradient-to-br from-white via-emerald-50 to-lime-50 p-5">
                  <div className="flex items-center justify-between">
                    <div>
                      <p className="text-sm font-medium text-emerald-900/55">
                        Smart listing flow
                      </p>
                      <h2 className="mt-1 text-2xl font-black tracking-tight text-emerald-950">
                        Bubble-first UX
                      </h2>
                    </div>

                    <div className="rounded-full bg-emerald-900 px-3 py-1 text-xs font-bold text-white">
                      Valora
                    </div>
                  </div>

                  <div className="mt-6 space-y-4">
                    <div className="rounded-[1.5rem] bg-white p-4 shadow-sm">
                      <div className="flex items-center gap-3">
                        <div className="flex h-12 w-12 items-center justify-center rounded-[1.2rem] bg-emerald-100">
                          <WandSparkles className="h-5 w-5 text-emerald-900" />
                        </div>
                        <div>
                          <p className="font-bold text-emerald-950">
                            Listing aanmaken
                          </p>
                          <p className="text-sm text-emerald-950/55">
                            Titel, categorie en features invullen
                          </p>
                        </div>
                      </div>
                    </div>

                    <div className="rounded-[1.5rem] bg-white p-4 shadow-sm">
                      <div className="flex items-center gap-3">
                        <div className="flex h-12 w-12 items-center justify-center rounded-[1.2rem] bg-lime-100">
                          <Sparkles className="h-5 w-5 text-lime-900" />
                        </div>
                        <div>
                          <p className="font-bold text-emerald-950">
                            AI prediction ophalen
                          </p>
                          <p className="text-sm text-emerald-950/55">
                            Direct gekoppeld aan je backend model-endpoints
                          </p>
                        </div>
                      </div>
                    </div>

                    <div className="rounded-[1.5rem] bg-white p-4 shadow-sm">
                      <div className="flex items-center gap-3">
                        <div className="flex h-12 w-12 items-center justify-center rounded-[1.2rem] bg-cyan-100">
                          <Search className="h-5 w-5 text-cyan-900" />
                        </div>
                        <div>
                          <p className="font-bold text-emerald-950">
                            Browse gepubliceerd aanbod
                          </p>
                          <p className="text-sm text-emerald-950/55">
                            Geen mockdata, maar echte gepubliceerde listings
                          </p>
                        </div>
                      </div>
                    </div>
                  </div>

                  <Link
                    href="/browse"
                    className="mt-6 inline-flex items-center gap-2 rounded-full bg-emerald-900 px-5 py-3 text-sm font-bold text-white transition hover:bg-emerald-800"
                  >
                    Open browse
                    <ArrowRight className="h-4 w-4" />
                  </Link>
                </div>
              </div>
            </div>
          </div>
        </section>

        <section id="features" className="mx-auto w-full max-w-7xl px-6 py-4 lg:px-8">
          <div className="mb-8 max-w-2xl">
            <p className="text-sm font-bold uppercase tracking-[0.22em] text-emerald-900/50">
              Features
            </p>
            <h2 className="mt-3 text-3xl font-black tracking-tight text-emerald-950 md:text-4xl">
              Zacht design, serieuze functionaliteit
            </h2>
          </div>

          <div className="grid gap-6 md:grid-cols-2 xl:grid-cols-4">
            {features.map((feature) => {
              const Icon = feature.icon;

              return (
                <article
                  key={feature.title}
                  className="soft-panel rounded-[2rem] p-6"
                >
                  <div className="flex h-14 w-14 items-center justify-center rounded-[1.4rem] bg-emerald-100">
                    <Icon className="h-6 w-6 text-emerald-900" />
                  </div>

                  <h3 className="mt-5 text-lg font-black text-emerald-950">
                    {feature.title}
                  </h3>

                  <p className="mt-3 text-sm leading-7 text-emerald-950/62">
                    {feature.description}
                  </p>
                </article>
              );
            })}
          </div>
        </section>
      </main>

      <Footer />
    </>
  );
}