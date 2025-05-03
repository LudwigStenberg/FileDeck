import { useEffect, useState } from "react";
import { FileList } from "../components/FileList";
import { FolderList } from "../components/FolderList";
import { Navbar } from "../components/Navbar";
import * as folderService from "../services/folderService";
import "../styles/navbar.css";
import { FolderResponse } from "../types";

export default function DashboardPage() {
  const [currentFolderId, setCurrentFolderId] = useState<Number | null>(null);
  const [isLoading, setIsLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const [folders, setFolders] = useState<FolderResponse[]>([]);

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
