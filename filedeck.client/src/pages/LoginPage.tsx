import { useState } from "react";
import { useNavigate, Link } from "react-router";
import { useAuth } from "../context/AuthContext";
import { Login } from "../components/auth/Login";

import "../index.css";
import"../styles/auth.css";

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
        navigate('/dashboard');
      } else {
        setError('Login failed. Please check your credentials.');
      }
    } catch (error) {
      console.error('Login error:', error);
      setError('An error occurred during login. Please try again.');
    } finally {
      setIsLoading(false);
    }
  };

  return (
    
    <div className="login-page">
      <h1>Login to Your Account</h1>

      {error && <div className="error-message">{error}</div>}

      <Login onSubmit={handleLogin} isLoading={isLoading}/>

      <p>
        Don't have an account? <Link to="/register">Register here</Link>
      </p>
    </div>
  )




}
