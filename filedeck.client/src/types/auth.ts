export interface RegisterRequestDto {
  email: string;
  password: string;
  confirmPassword: string;
}

export interface RegisterResponseDto {
  succeeded: boolean;
  userId: string;
  username: string;
  email: string;
  errors: string[];
}

export interface LoginRequestDto {
  email: string;
  password: string;
}

export interface LoginResponseDto {
  succeeded: boolean;
  token: string;
  expiration: string;
  userId: string;
  username: string;
  email: string;
  errors: string[];
}
