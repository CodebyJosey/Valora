export interface ListingResponse {
  id: string;
  ownerUserId: string;
  title: string;
  description: string;
  categoryKey: string;
  askingPrice: number;
  predictedPrice: number | null;
  soldPrice: number | null;
  soldAtUtc: string | null;
  status: string;
  featuresJson: string;
  createdAtUtc: string;
  updatedAtUtc: string;
}

export interface CreateListingRequest {
  title: string;
  description: string;
  categoryKey: string;
  askingPrice: number;
  featuresJson: string;
}

export interface UpdateListingRequest {
  title: string;
  description: string;
  askingPrice: number;
  featuresJson: string;
}