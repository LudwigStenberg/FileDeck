import { useState } from "react";
import { useNavigate } from "react-router";
import { useAuth } from "../context/AuthContext";
import { Login } from "../components/auth/Login";

import "../styles/auth.css";
import logo from "../assets/logo.png";

export default function LoginPage() {
  const [isLoading, setIsLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);

  const { login } = useAuth();
  const navigate = useNavigate();

  const handleLogin = async (email: string, password: string) => {
    setIsLoading(true);
    setError(null);

    try {
      const success = await login({ email, password });
      if (success) {
        navigate("/dashboard");
      } else {
        setError("Login failed. Please check your credentials.");
      }
    } catch (error) {
      console.error("Login error:", error);
      setError("An error occurred during login. Please try again.");
    } finally {
      setIsLoading(false);
    }
  };

  return (
    <div className="login-page">
      <img src={logo} alt="FileDeck Logo" className="app-logo" />
      <h1 className="auth-header">Login to Your Account</h1>
      {error && <div className="error-message">{error}</div>}
      <Login onSubmit={handleLogin} isLoading={isLoading} />
    </div>
  );
}
