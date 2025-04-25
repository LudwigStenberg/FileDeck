
// This component should be imported on the LoginPage
// Responsible for rendering the Registration UI

import React, { useState } from "react";

interface RegisterProps {
    onSubmit: (email: string, password: string, confirmPassword: string) => Promise<void>;
    isLoading: boolean;
}

export function Register({onSubmit, isLoading}: RegisterProps) {
    const [email, setEmail] = useState("");
    const [password, setPassword] = useState("");
    const [confirmPassword, setConfirmPassword] = useState("");

    const handleSubmit = async (e: React.FormEvent) => {
        e.preventDefault();
        await onSubmit(email, password, confirmPassword);
    };

    return (
        <form onSubmit={handleSubmit}>
            <div className="form-group">
                <label htmlFor="email">Email:</label>
                <input
                type="email"
                id="email"
                value={email}
                onChange={(e) => setEmail(e.target.value)}
                required
                 />
            </div>
            <div className="form-group">    
                <label htmlFor="password">Password:</label>
                <input
                type="password"
                id="password"
                value={password}
                onChange={(e) => setPassword(e.target.value)}
                required
                />
            </div>
            <div className="form-group">
                <label htmlFor="confirmPassword">Confirm Password:</label>
                <input
                type="password"
                id="confirmPassword"
                value={confirmPassword}
                onChange={(e) => setConfirmPassword(e.target.value)}
                />
            </div>
           
            <button type="submit" disabled={isLoading}>
                {isLoading ? "Registering..." : "Register"}
            </button>
        </form>
    )
}