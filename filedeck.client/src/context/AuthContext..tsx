import { createContext, ReactNode, useEffect, useState } from "react";
import { LoginRequestDto, RegisterRequestDto } from "../types";
import * as authService from "../services/authService";

interface AuthContextType {
  isAuthenticated: boolean;
  userId: string | null;
  isLoading: boolean;
  login: (data: LoginRequestDto) => Promise<boolean>;
  register: (data: RegisterRequestDto) => Promise<boolean>;
  logout: () => void;
}

const AuthContext = createContext<AuthContextType | undefined>(undefined);

interface AuthProviderProps {
  children: ReactNode;
}

export const AuthProvider = ({ children }: AuthProviderProps) => {
  const [isAuthenticated, setIsAuthenticated] = useState(false);
  const [userId, setUserId] = useState<string | null>(null);
  const [isLoading, setIsLoading] = useState(true);

  useEffect(() => {
    const token = localStorage.getItem("token");
    const storedUserId = localStorage.getItem("userId");

    if (token && storedUserId) {
      setIsAuthenticated(true);
      setUserId(storedUserId);
    }

    setIsLoading(false);
  }, []);

  const login = async (loginData: LoginRequestDto): Promise<boolean> => {
    try {
      const response = await authService.login(loginData);

      if (response.succeeded) {
        setIsAuthenticated(true);
        setUserId(response.userId);
        return true;
      }

      return false;
    } catch (error) {
      console.error("Login error:", error);
      return false;
    }
  };
};
