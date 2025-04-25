import { useState } from "react";

import "../index.css";
import { useAuth } from "../context/AuthContext";
import { Link, useNavigate } from "react-router";
import { Register } from "../components/auth/Register";

export default function RegisterPage() {
  const [isLoading, setIsLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);

  const { register } = useAuth();
  const navigate = useNavigate();

  const handleRegister = async (email: string, password: string, confirmPassword: string) =>{
    
    if (password !== confirmPassword){
      setError("Passwords do not match");
      return;
    }
    
    setIsLoading(true);
    setError(null);
  
  try {
    const success = await register({
      email, 
      password, 
      confirmPassword
    });
    
    if (success)
    {
      navigate("/login");
    } else {
      setError("Registration failed. Please try again.")
    } 
  } catch (error) {
    console.error("Registration error:", error);
    setError("An error occurred during registration. Please try again.")
  } finally {
    setIsLoading(false);
  }
  };

  return ( 
    <div className="register-page">
      <h2>Register a New Account</h2>

      {error && <div className="error-message">{error}</div>}

      <Register onSubmit={handleRegister} isLoading={isLoading}/>

      <p>
        Already have an account? <Link to="/Login">Login here</Link>
      </p>
    </div>
  )
}
