export const appConfig = {
  name: "Valora",
  description: "AI-powered marketplace pricing for smarter listings.",
  apiBaseUrl: process.env.NEXT_PUBLIC_API_BASE_URL ?? "",
} as const;

if (!appConfig.apiBaseUrl) {
  throw new Error("NEXT_PUBLIC_API_BASE_URL is not configured.");
}