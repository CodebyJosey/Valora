"use client";

import { useEffect } from "react";
import { usePathname, useRouter } from "next/navigation";
import { getAccessToken } from "@/lib/auth/token-storage";
import { useCurrentUser } from "@/hooks/use-current-user";

interface AuthGuardProps {
  children: React.ReactNode;
}

export function AuthGuard({ children }: AuthGuardProps): React.JSX.Element {
  const router = useRouter();
  const pathname = usePathname();
  const token = getAccessToken();
  const { isAuthenticated, isLoading } = useCurrentUser();

  useEffect(() => {
    if (!token) {
      const returnUrl = encodeURIComponent(pathname ?? "/dashboard");
      router.replace(`/login?returnUrl=${returnUrl}`);
    }
  }, [pathname, router, token]);

  useEffect(() => {
    if (!isLoading && token && !isAuthenticated) {
      const returnUrl = encodeURIComponent(pathname ?? "/dashboard");
      router.replace(`/login?returnUrl=${returnUrl}`);
    }
  }, [isAuthenticated, isLoading, pathname, router, token]);

  if (!token || isLoading || !isAuthenticated) {
    return (
      <main className="mx-auto flex min-h-[60vh] w-full max-w-7xl items-center justify-center px-6 py-16 lg:px-8">
        <div className="soft-panel rounded-[2rem] p-8 text-center">
          <h1 className="text-2xl font-black text-emerald-950">
            Bezig met controleren...
          </h1>
          <p className="mt-3 text-sm leading-7 text-emerald-950/65">
            We checken of je bent ingelogd.
          </p>
        </div>
      </main>
    );
  }

  return <>{children}</>;
}