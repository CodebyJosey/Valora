import { apiRequest } from "@/lib/api/api-client";
import type {
  CreateListingRequest,
  ListingResponse,
  UpdateListingRequest,
} from "@/types/listings";

export const listingsApi = {
  getPublished(signal?: AbortSignal): Promise<ListingResponse[]> {
    return apiRequest<ListingResponse[]>("/api/listings", {
      method: "GET",
      signal,
    });
  },

  getById(id: string, signal?: AbortSignal): Promise<ListingResponse> {
    return apiRequest<ListingResponse>(`/api/listings/${id}`, {
      method: "GET",
      signal,
    });
  },

  getMine(accessToken: string, signal?: AbortSignal): Promise<ListingResponse[]> {
    return apiRequest<ListingResponse[]>("/api/listings/mine", {
      method: "GET",
      accessToken,
      signal,
    });
  },

  create(
    request: CreateListingRequest,
    accessToken: string,
    signal?: AbortSignal,
  ): Promise<ListingResponse> {
    return apiRequest<ListingResponse>("/api/listings", {
      method: "POST",
      body: request,
      accessToken,
      signal,
    });
  },

  update(
    id: string,
    request: UpdateListingRequest,
    accessToken: string,
    signal?: AbortSignal,
  ): Promise<ListingResponse> {
    return apiRequest<ListingResponse>(`/api/listings/${id}`, {
      method: "PUT",
      body: request,
      accessToken,
      signal,
    });
  },

  delete(id: string, accessToken: string, signal?: AbortSignal): Promise<void> {
    return apiRequest<void>(`/api/listings/${id}`, {
      method: "DELETE",
      accessToken,
      signal,
    });
  },

  predictPrice(
    id: string,
    accessToken: string,
    signal?: AbortSignal,
  ): Promise<void> {
    return apiRequest<void>(`/api/listings/${id}/predict-price`, {
      method: "POST",
      accessToken,
      signal,
    });
  },

  publish(id: string, accessToken: string, signal?: AbortSignal): Promise<void> {
    return apiRequest<void>(`/api/listings/${id}/publish`, {
      method: "POST",
      accessToken,
      signal,
    });
  },

  unpublish(
    id: string,
    accessToken: string,
    signal?: AbortSignal,
  ): Promise<void> {
    return apiRequest<void>(`/api/listings/${id}/unpublish`, {
      method: "POST",
      accessToken,
      signal,
    });
  },
};