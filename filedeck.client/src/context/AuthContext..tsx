import {
  createContext,
  ReactNode,
  useContext,
  useEffect,
  useState,
} from "react";
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

  const register = async (
    registerData: RegisterRequestDto
  ): Promise<boolean> => {
    try {
      const response = await authService.register(registerData);
      return response.succeeded;
    } catch (error) {
      console.error("Registration error:", error);
      return false;
    }
  };

  const logout = () => {
    authService.logout();
    setIsAuthenticated(false);
    setUserId(null);
  };

  const contextValue: AuthContextType = {
    isAuthenticated,
    userId,
    isLoading,
    login,
    register,
    logout,
  };

  return (
    <AuthContext.Provider value={contextValue}>{children}</AuthContext.Provider>
  );
};

export const useAuth = () => {
  const context = useContext(AuthContext);

  if (context === undefined) {
    throw new Error("useAuth must be used within an AuthProvider");
  }

  return context;
};
