import api from "./api";
import {
  RegisterRequest,
  RegisterResponse,
  LoginRequest,
  LoginResponse,
} from "../types";

// Register User
export const register = async (
  registerData: RegisterRequest
): Promise<RegisterResponse> => {
  const response = await api.post<RegisterResponse>(
    "/auth/register",
    registerData
  );
  return response.data;
};

// Login
export const login = async (
  loginData: LoginRequest
): Promise<LoginResponse> => {
  const response = await api.post<LoginResponse>("auth/login", loginData);

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
