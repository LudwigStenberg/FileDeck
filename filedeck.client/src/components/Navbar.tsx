import { useAuth } from "../context/AuthContext";

import logo from "../assets/logo.png";
import { Link } from "react-router";

export const Navbar = () => {

    return (
        <div className="navbar-container">
        	<img src={logo} alt="FileDeck Logo" className="navbar-logo"/>
          <ul className="navbar-list">
            <li><Link to="/dashboard">Dashboard</Link></li>
						<li><Link to="/profile">Profile</Link></li>
          </ul>
        </div>
    );
}