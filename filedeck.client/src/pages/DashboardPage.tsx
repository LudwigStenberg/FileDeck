import { useEffect, useState } from "react";
import { FileList } from "../components/FileList";
import { FolderList } from "../components/FolderList";
import { Navbar } from "../components/Navbar";
import { FolderResponse } from "../types";
import { CreateFolderModal } from "../components/CreateFolderModal";
import * as folderService from "../services/folderService";
import "../styles/navbar.css";
import "../styles/dashboard.css";

export default function DashboardPage() {
  const [currentFolderId, setCurrentFolderId] = useState<number | null>(null);
  const [isLoading, setIsLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const [folders, setFolders] = useState<FolderResponse[]>([]);
  const [isCreateFolderModalOpen, setIsCreateFolderModalOpen] = useState(false);

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

  useEffect(() => {
    fetchAllFolders();
  }, []);

  const handleFolderCreated = () => {
    fetchAllFolders();
  };

  return (
    <div>
      <Navbar />
      <div className="dashboard-content">
        {isLoading && <div className="loading-message">Loading folders...</div>}
        {error && <div className="error-message">{error}</div>}

        <h2>Folders & Files</h2>
        <div className="dashboard-actions">
          <button
            className="create-folder-button"
            onClick={() => setIsCreateFolderModalOpen(true)}
          >
            Create New Folder
          </button>
        </div>

        <CreateFolderModal
          currentFolderId={currentFolderId}
          onFolderCreated={handleFolderCreated}
          onClose={() => setIsCreateFolderModalOpen(false)}
          isOpen={isCreateFolderModalOpen}
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
