export interface RegisterRequest {
  email: string;
  password: string;
  confirmPassword: string;
}

export interface RegisterResponse {
  succeeded: boolean;
  userId: string;
  username: string;
  email: string;
  errors: string[];
}

export interface LoginRequest {
  email: string;
  password: string;
}

export interface LoginResponse {
  succeeded: boolean;
  token: string;
  expiration: string;
  userId: string;
  username: string;
  email: string;
  errors: string[];
}
