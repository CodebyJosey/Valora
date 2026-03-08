import { appConfig } from "@/lib/config";

type HttpMethod = "GET" | "POST" | "PUT" | "DELETE";

interface ApiRequestOptions {
  method?: HttpMethod;
  body?: unknown;
  accessToken?: string | null;
  signal?: AbortSignal;
}

interface ProblemDetailsResponse {
  title?: string;
  detail?: string;
  status?: number;
}

export class ApiError extends Error {
  public readonly status: number;
  public readonly detail?: string;

  public constructor(message: string, status: number, detail?: string) {
    super(message);
    this.name = "ApiError";
    this.status = status;
    this.detail = detail;
  }
}

export async function apiRequest<T>(
  endpoint: string,
  options: ApiRequestOptions = {},
): Promise<T> {
  const { method = "GET", body, accessToken, signal } = options;

  const headers = new Headers();
  headers.set("Accept", "application/json");

  if (body !== undefined) {
    headers.set("Content-Type", "application/json");
  }

  if (accessToken) {
    headers.set("Authorization", `Bearer ${accessToken}`);
  }

  const response = await fetch(`${appConfig.apiBaseUrl}${endpoint}`, {
    method,
    headers,
    body: body !== undefined ? JSON.stringify(body) : undefined,
    signal,
  });

  if (!response.ok) {
    let title = "Request failed";
    let detail: string | undefined;

    try {
      const problem = (await response.json()) as ProblemDetailsResponse;
      title = problem.title ?? title;
      detail = problem.detail;
    } catch {
      // Ignore invalid JSON error bodies.
    }

    throw new ApiError(title, response.status, detail);
  }

  if (response.status === 204) {
    return undefined as T;
  }

  return (await response.json()) as T;
}