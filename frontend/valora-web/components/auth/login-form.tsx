"use client";

import { useState } from "react";
import Link from "next/link";
import { useRouter, useSearchParams } from "next/navigation";
import { useMutation, useQueryClient } from "@tanstack/react-query";
import { useForm } from "react-hook-form";
import { z } from "zod";
import { zodResolver } from "@hookform/resolvers/zod";
import { AlertTriangle, LoaderCircle, LogIn } from "lucide-react";
import { authApi } from "@/lib/api/auth-api";
import { setAccessToken } from "@/lib/auth/token-storage";
import type { LoginRequest } from "@/types/auth";

const loginSchema = z.object({
  email: z.email("Vul een geldig e-mailadres in."),
  password: z.string().min(1, "Vul je wachtwoord in."),
});

type LoginFormValues = z.infer<typeof loginSchema>;

export function LoginForm(): React.JSX.Element {
  const router = useRouter();
  const searchParams = useSearchParams();
  const queryClient = useQueryClient();
  const [serverError, setServerError] = useState<string | null>(null);

  const form = useForm<LoginFormValues>({
    resolver: zodResolver(loginSchema),
    defaultValues: {
      email: "",
      password: "",
    },
  });

  const loginMutation = useMutation({
    mutationFn: (request: LoginRequest) => authApi.login(request),
    onSuccess: async (response) => {
      setAccessToken(response.accessToken);

      await queryClient.invalidateQueries({
        queryKey: ["current-user"],
      });

      const returnUrl = searchParams.get("returnUrl");
      router.push(returnUrl || "/dashboard");
    },
    onError: () => {
      setServerError("Inloggen is mislukt. Controleer je gegevens en probeer opnieuw.");
    },
  });

  function onSubmit(values: LoginFormValues): void {
    setServerError(null);
    loginMutation.mutate(values);
  }

  return (
    <div className="soft-panel w-full max-w-md rounded-[2rem] p-8">
      <p className="text-sm font-bold uppercase tracking-[0.22em] text-emerald-900/50">
        Auth
      </p>

      <h1 className="mt-3 text-3xl font-black tracking-tight text-emerald-950">
        Inloggen
      </h1>

      <p className="mt-3 text-sm leading-7 text-emerald-950/65">
        Log in om je dashboard, listings en predictions te beheren.
      </p>

      <form onSubmit={form.handleSubmit(onSubmit)} className="mt-8 space-y-5">
        <div>
          <label
            htmlFor="email"
            className="mb-2 block text-sm font-bold text-emerald-950"
          >
            E-mailadres
          </label>
          <input
            id="email"
            type="email"
            autoComplete="email"
            {...form.register("email")}
            className="w-full rounded-[1.2rem] border border-emerald-200 bg-white px-4 py-3 text-sm text-emerald-950 shadow-sm outline-none transition focus:border-emerald-400"
          />
          {form.formState.errors.email ? (
            <p className="mt-2 text-sm text-red-600">
              {form.formState.errors.email.message}
            </p>
          ) : null}
        </div>

        <div>
          <label
            htmlFor="password"
            className="mb-2 block text-sm font-bold text-emerald-950"
          >
            Wachtwoord
          </label>
          <input
            id="password"
            type="password"
            autoComplete="current-password"
            {...form.register("password")}
            className="w-full rounded-[1.2rem] border border-emerald-200 bg-white px-4 py-3 text-sm text-emerald-950 shadow-sm outline-none transition focus:border-emerald-400"
          />
          {form.formState.errors.password ? (
            <p className="mt-2 text-sm text-red-600">
              {form.formState.errors.password.message}
            </p>
          ) : null}
        </div>

        {serverError ? (
          <div className="rounded-[1.2rem] border border-red-200 bg-red-50 p-4 text-sm text-red-700">
            <div className="flex items-start gap-3">
              <AlertTriangle className="mt-0.5 h-4 w-4" />
              <span>{serverError}</span>
            </div>
          </div>
        ) : null}

        <button
          type="submit"
          disabled={loginMutation.isPending}
          className="inline-flex w-full items-center justify-center gap-2 rounded-full bg-emerald-900 px-5 py-3 text-sm font-bold text-white transition hover:bg-emerald-800 disabled:cursor-not-allowed disabled:opacity-70"
        >
          {loginMutation.isPending ? (
            <>
              <LoaderCircle className="h-4 w-4 animate-spin" />
              Bezig met inloggen...
            </>
          ) : (
            <>
              <LogIn className="h-4 w-4" />
              Inloggen
            </>
          )}
        </button>
      </form>

      <p className="mt-6 text-sm text-emerald-950/65">
        Nog geen account?{" "}
        <Link href="/register" className="font-bold text-emerald-950">
          Registreer hier
        </Link>
      </p>
    </div>
  );
}