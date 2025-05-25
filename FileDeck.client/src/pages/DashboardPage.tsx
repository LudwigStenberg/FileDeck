import { useEffect, useState } from "react";
import { FileList } from "../components/FileList";
import { FolderList } from "../components/FolderList";
import { Navbar } from "../components/Navbar";
import { FileResponse, FolderResponse } from "../types";
import { CreateFolderModal } from "../components/CreateFolderModal";
import * as folderService from "../services/folderService";
import * as fileService from "../services/fileService";
import "../styles/navbar.css";
import "../styles/dashboard.css";
import { FileUploadModal } from "../components/FileUploadModal";

export default function DashboardPage() {
  const [currentFolderId, setCurrentFolderId] = useState<number | null>(null);
  const [isLoading, setIsLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const [folders, setFolders] = useState<FolderResponse[]>([]);
  const [files, setFiles] = useState<FileResponse[]>([]);
  const [isCreateFolderModalOpen, setIsCreateFolderModalOpen] = useState(false);
  const [isFileUploadModalOpen, setIsFileUploadModalOpen] = useState(false);

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

  const fetchFiles = async () => {
    try {
      setIsLoading(true);
      let files: FileResponse[];

      if (currentFolderId === null) {
        files = await fileService.getRootFiles();
      } else {
        files = await folderService.getFilesInFolder(currentFolderId);
      }

      setFiles(files);
      setError(null);
    } catch (error) {
      console.error("Error fetching files:", error);
      setError("Failed to load files. Please try again later");
    } finally {
      setIsLoading(false);
    }
  };

  useEffect(() => {
    fetchAllFolders();
  }, []);

  useEffect(() => {
    fetchFiles();
  }, [currentFolderId]);

  const handleFolderChanged = () => {
    fetchAllFolders();
  };

  const handleFileUploaded = () => {
    fetchFiles();
  };

  return (
    <div className="dashboard-container">
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
          <button
            className="action-button upload-button"
            onClick={() => setIsFileUploadModalOpen(true)}
          >
            Upload File
          </button>
        </div>

        <CreateFolderModal
          currentFolderId={currentFolderId}
          onFolderCreated={handleFolderChanged}
          onClose={() => setIsCreateFolderModalOpen(false)}
          isOpen={isCreateFolderModalOpen}
        />

        <FileUploadModal
          currentFolderId={currentFolderId}
          onFileUploaded={handleFileUploaded}
          onClose={() => setIsFileUploadModalOpen(false)}
          isOpen={isFileUploadModalOpen}
        />
        <FolderList
          folders={folders}
          currentFolderId={currentFolderId}
          setCurrentFolderId={setCurrentFolderId}
          onFolderDeleted={handleFolderChanged}
        />
        <FileList files={files} onFileDeleted={fetchFiles} />
      </div>
    </div>
  );
}
