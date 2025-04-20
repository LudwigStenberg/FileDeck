import { LoginRequestDto, RegisterRequestDto } from "../types";

interface AuthContextType {
  isAuthenticated: boolean;
  userId: string | null;
  isLoading: boolean;
  login: (data: LoginRequestDto) => Promise<boolean>;
  register: (data: RegisterRequestDto) => Promise<boolean>;
  logout: () => void;
}
