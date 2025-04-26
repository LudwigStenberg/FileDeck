import { FileList } from "../components/FileList";
import { Navbar } from "../components/Navbar";

import "../styles/navbar.css";

export default function DashboardPage() {
  return (
    <div>
      <Navbar />
      <div className="dashboard-content">
        <FileList />
      </div>
    </div>
  );
}
