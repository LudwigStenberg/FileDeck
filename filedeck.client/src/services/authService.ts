import api from "./api";
import {
  RegisterRequestDto,
  RegisterResponseDto,
  LoginRequestDto,
  LoginResponseDto,
} from "../types";

// Register User
export const register = async (
  registerData: RegisterRequestDto
): Promise<RegisterResponseDto> => {
  const response = await api.post<RegisterResponseDto>(
    "/auth/register",
    registerData
  );
  return response.data;
};

// Login
export const login = async (
  loginData: LoginRequestDto
): Promise<LoginResponseDto> => {
  const response = await api.post<LoginResponseDto>("auth/login", loginData);

  if (response.data.succeeded && response.data.token) {
    localStorage.setItem("token", response.data.token);
    localStorage.setItem("userId", response.data.userId);
  }

  return response.data;
};

// Logout
export const logout = (): void => {
  localStorage.removeItem("token");
  localStorage.removeItem("userId");
};
