import { useState } from "react";
import { FileList } from "../components/FileList";
import { FolderList } from "../components/FolderList";
import { Navbar } from "../components/Navbar";
import "../styles/navbar.css";

export default function DashboardPage() {
  const [currentFolderId, setCurrentFolderId] = useState<Number | null>(null);
  return (
    <div>
      <Navbar />
      <div className="dashboard-content">
        <FolderList />
        <FileList />
      </div>
    </div>
  );
}
