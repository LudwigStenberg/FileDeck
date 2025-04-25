import { useAuth } from "../context/AuthContext";

import logo from "../assets/logo.png";

export const Navbar = () => {

    return (
        <img src={logo} alt="FileDeck Logo" className="navbar-logo"/>
    );
}