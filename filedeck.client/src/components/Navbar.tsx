import { useAuth } from "../context/AuthContext";

import logo from "../assets/logo.png";

export const Navbar = () => {

    return (
        <div className="navbar-container">
        	<img src={logo} alt="FileDeck Logo" className="navbar-logo"/>
          <ul className="navbar-list">
            <li>Dashboard</li>
						<li>Profile</li>
          </ul>
        </div>
    );
}