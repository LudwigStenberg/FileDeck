import React, { ReactEventHandler, useState } from "react";

interface LoginProps {
    onSubmit: (email: string, password: string) => Promise<void>;
    isLoading?: boolean;
}

export function Login({onSubmit, isLoading = false }: LoginProps) {
    const [email, setEmail] = useState("");
    const [password, setPassword] = useState("");

    const handleSubmit = async (e: React.FormEvent) => {
        e.preventDefault();
        await onSubmit(email, password);
    };

    return (
        <form onSubmit={handleSubmit}>
            <div className="form-group">
                <label htmlFor="email">Email:</label>
                <input 
                type="email"
                id="email"
                value="{email}"
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
            <button type="submit" disabled={isLoading}>
                {isLoading ? "Logging in..." : "Login"}
            </button>
        </form>
    )
}