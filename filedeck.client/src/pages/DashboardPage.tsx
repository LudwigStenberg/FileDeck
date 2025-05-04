import { useEffect, useState } from "react";
import { FileList } from "../components/FileList";
import { FolderList } from "../components/FolderList";
import { Navbar } from "../components/Navbar";
import * as folderService from "../services/folderService";
import "../styles/navbar.css";
import { FolderResponse } from "../types";
import { Breadcrumb } from "../components/Breadcrumb";

export default function DashboardPage() {
  const [currentFolderId, setCurrentFolderId] = useState<number | null>(null);
  const [isLoading, setIsLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const [folders, setFolders] = useState<FolderResponse[]>([]);

  useEffect(() => {
    const fetchAllFolders = async () => {
      try {
        setIsLoading(true);
        const folders = await folderService.getAllFolders();
        setFolders(folders);
      } catch (error) {
        console.error("Error fetching all folders:", error);
        setError("Failed to load all folders. Please try again later");
      } finally {
        setIsLoading(false);
      }
    };

    fetchAllFolders();
  }, []);

  return (
    <div>
      <Navbar />
      <div className="dashboard-content">
        {isLoading && <div className="loading-message">Loading folders...</div>}
        {error && <div className="error-message">{error}</div>}
        <Breadcrumb
          folders={folders}
          currentFolderId={currentFolderId as number | null}
          setCurrentFolderId={setCurrentFolderId}
        />
        <FolderList
          folders={folders}
          currentFolderId={currentFolderId}
          setCurrentFolderId={setCurrentFolderId}
        />
        <FileList />
      </div>
    </div>
  );
}
