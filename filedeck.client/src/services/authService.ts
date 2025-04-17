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
    "auth/register",
    registerData
  );
  return response.data;
};
// Login
// Logout
