import { apiRequest } from "@/lib/api/api-client";
import type {
  AuthResponse,
  CurrentUserResponse,
  LoginRequest,
  RegisterRequest,
} from "@/types/auth";

export const authApi = {
  login(request: LoginRequest, signal?: AbortSignal): Promise<AuthResponse> {
    return apiRequest<AuthResponse>("/api/auth/login", {
      method: "POST",
      body: request,
      signal,
    });
  },

  register(request: RegisterRequest, signal?: AbortSignal): Promise<void> {
    return apiRequest<void>("/api/auth/register", {
      method: "POST",
      body: request,
      signal,
    });
  },

  me(accessToken: string, signal?: AbortSignal): Promise<CurrentUserResponse> {
    return apiRequest<CurrentUserResponse>("/api/auth/me", {
      method: "GET",
      accessToken,
      signal,
    });
  },
};