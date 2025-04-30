import { FileList } from "../components/FileList";
import { FolderList } from "../components/FolderList";
import { Navbar } from "../components/Navbar";

import "../styles/navbar.css";

export default function DashboardPage() {
  return (
    <div>
      <Navbar />
      <div className="dashboard-content">
        <FileList />
        <FolderList />
      </div>
    </div>
  );
}
