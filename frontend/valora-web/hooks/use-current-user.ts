"use client";

import { useQuery } from "@tanstack/react-query";
import { authApi } from "@/lib/api/auth-api";
import { getAccessToken } from "@/lib/auth/token-storage";
import type { CurrentUserResponse } from "@/types/auth";

interface UseCurrentUserResult {
  user: CurrentUserResponse | null;
  isAuthenticated: boolean;
  isLoading: boolean;
  isError: boolean;
  refetch: () => void;
}

export function useCurrentUser(): UseCurrentUserResult {
  const token = getAccessToken();

  const query = useQuery({
    queryKey: ["current-user", token],
    queryFn: ({ signal }) => authApi.me(token as string, signal),
    enabled: Boolean(token),
    staleTime: 60_000,
    retry: false,
  });

  return {
    user: query.data ?? null,
    isAuthenticated: Boolean(token) && Boolean(query.data),
    isLoading: Boolean(token) && query.isLoading,
    isError: query.isError,
    refetch: () => {
      void query.refetch();
    },
  };
}