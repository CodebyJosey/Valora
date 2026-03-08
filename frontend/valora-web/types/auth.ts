export type UserRole = "Buyer" | "Seller" | "Admin";

export interface LoginRequest {
  email: string;
  password: string;
}

export interface RegisterRequest {
  displayName: string;
  email: string;
  password: string;
  role: Exclude<UserRole, "Admin">;
}

export interface AuthResponse {
  accessToken: string;
  expiresAtUtc: string;
  userId: string;
  displayName: string;
  email: string;
  roles: string[];
}

export interface CurrentUserResponse {
  userId: string;
  displayName: string;
  email: string;
  roles: string[];
}