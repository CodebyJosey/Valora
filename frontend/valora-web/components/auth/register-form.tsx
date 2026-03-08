"use client";

import { useState } from "react";
import Link from "next/link";
import { useRouter } from "next/navigation";
import { useMutation } from "@tanstack/react-query";
import { useForm } from "react-hook-form";
import { z } from "zod";
import { zodResolver } from "@hookform/resolvers/zod";
import { AlertTriangle, LoaderCircle, UserPlus } from "lucide-react";
import { authApi } from "@/lib/api/auth-api";
import type { RegisterRequest } from "@/types/auth";

const registerSchema = z
  .object({
    displayName: z.string().min(2, "Vul minimaal 2 tekens in."),
    email: z.email("Vul een geldig e-mailadres in."),
    password: z
      .string()
      .min(8, "Je wachtwoord moet minimaal 8 tekens bevatten."),
    confirmPassword: z.string().min(1, "Bevestig je wachtwoord."),
    role: z.enum(["Buyer", "Seller"]),
  })
  .refine((data) => data.password === data.confirmPassword, {
    message: "De wachtwoorden komen niet overeen.",
    path: ["confirmPassword"],
  });

type RegisterFormValues = z.infer<typeof registerSchema>;

export function RegisterForm(): React.JSX.Element {
  const router = useRouter();
  const [serverError, setServerError] = useState<string | null>(null);

  const form = useForm<RegisterFormValues>({
    resolver: zodResolver(registerSchema),
    defaultValues: {
      displayName: "",
      email: "",
      password: "",
      confirmPassword: "",
      role: "Seller",
    },
  });

  const registerMutation = useMutation({
    mutationFn: (request: RegisterRequest) => authApi.register(request),
    onSuccess: () => {
      router.push("/login");
    },
    onError: () => {
      setServerError("Registreren is mislukt. Controleer je gegevens en probeer opnieuw.");
    },
  });

  function onSubmit(values: RegisterFormValues): void {
    setServerError(null);

    registerMutation.mutate({
      displayName: values.displayName,
      email: values.email,
      password: values.password,
      role: values.role,
    });
  }

  return (
    <div className="soft-panel w-full max-w-md rounded-[2rem] p-8">
      <p className="text-sm font-bold uppercase tracking-[0.22em] text-emerald-900/50">
        Auth
      </p>

      <h1 className="mt-3 text-3xl font-black tracking-tight text-emerald-950">
        Registreren
      </h1>

      <p className="mt-3 text-sm leading-7 text-emerald-950/65">
        Maak een account aan om Valora te gebruiken als buyer of seller.
      </p>

      <form onSubmit={form.handleSubmit(onSubmit)} className="mt-8 space-y-5">
        <div>
          <label
            htmlFor="displayName"
            className="mb-2 block text-sm font-bold text-emerald-950"
          >
            Weergavenaam
          </label>
          <input
            id="displayName"
            type="text"
            autoComplete="name"
            {...form.register("displayName")}
            className="w-full rounded-[1.2rem] border border-emerald-200 bg-white px-4 py-3 text-sm text-emerald-950 shadow-sm outline-none transition focus:border-emerald-400"
          />
          {form.formState.errors.displayName ? (
            <p className="mt-2 text-sm text-red-600">
              {form.formState.errors.displayName.message}
            </p>
          ) : null}
        </div>

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
            htmlFor="role"
            className="mb-2 block text-sm font-bold text-emerald-950"
          >
            Rol
          </label>
          <select
            id="role"
            {...form.register("role")}
            className="w-full rounded-[1.2rem] border border-emerald-200 bg-white px-4 py-3 text-sm text-emerald-950 shadow-sm outline-none transition focus:border-emerald-400"
          >
            <option value="Seller">Seller</option>
            <option value="Buyer">Buyer</option>
          </select>
          {form.formState.errors.role ? (
            <p className="mt-2 text-sm text-red-600">
              {form.formState.errors.role.message}
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
            autoComplete="new-password"
            {...form.register("password")}
            className="w-full rounded-[1.2rem] border border-emerald-200 bg-white px-4 py-3 text-sm text-emerald-950 shadow-sm outline-none transition focus:border-emerald-400"
          />
          {form.formState.errors.password ? (
            <p className="mt-2 text-sm text-red-600">
              {form.formState.errors.password.message}
            </p>
          ) : null}
        </div>

        <div>
          <label
            htmlFor="confirmPassword"
            className="mb-2 block text-sm font-bold text-emerald-950"
          >
            Bevestig wachtwoord
          </label>
          <input
            id="confirmPassword"
            type="password"
            autoComplete="new-password"
            {...form.register("confirmPassword")}
            className="w-full rounded-[1.2rem] border border-emerald-200 bg-white px-4 py-3 text-sm text-emerald-950 shadow-sm outline-none transition focus:border-emerald-400"
          />
          {form.formState.errors.confirmPassword ? (
            <p className="mt-2 text-sm text-red-600">
              {form.formState.errors.confirmPassword.message}
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
          disabled={registerMutation.isPending}
          className="inline-flex w-full items-center justify-center gap-2 rounded-full bg-emerald-900 px-5 py-3 text-sm font-bold text-white transition hover:bg-emerald-800 disabled:cursor-not-allowed disabled:opacity-70"
        >
          {registerMutation.isPending ? (
            <>
              <LoaderCircle className="h-4 w-4 animate-spin" />
              Account aanmaken...
            </>
          ) : (
            <>
              <UserPlus className="h-4 w-4" />
              Registreren
            </>
          )}
        </button>
      </form>

      <p className="mt-6 text-sm text-emerald-950/65">
        Heb je al een account?{" "}
        <Link href="/login" className="font-bold text-emerald-950">
          Log hier in
        </Link>
      </p>
    </div>
  );
}