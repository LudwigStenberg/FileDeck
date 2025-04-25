import { Link, useNavigate } from "react-router";
import { useAuth } from "../context/AuthContext";
import logo from "../assets/logo.png";
import { logout } from "../services/authService";


export const Navbar = () => {
		const  { userId, isAuthenticated, logout } = useAuth();
		const navigate = useNavigate();

    return (
        <div className="navbar-container">
        	<img src={logo} alt="FileDeck Logo" className="navbar-logo"/>
          <ul className="navbar-list">
            <li><Link to="/dashboard" className="navbar-link">Dashboard</Link></li>
						<li><Link to="/profile" className="navbar-link">Profile</Link></li>
          </ul>

					<div className="user-controls">
						{isAuthenticated && (
							<>
							<button
								className="logout-button"
								onClick={() => {
									logout();
									navigate("/");

								}}
								>Logout</button>
							</>
						)}

					</div>
        </div>
    );
}