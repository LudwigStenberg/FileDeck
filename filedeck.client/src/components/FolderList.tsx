import { useEffect, useState } from "react";
import { FolderResponse } from "../types";
import * as folderService from "../services/folderService";
import { FaRegFolder } from "react-icons/fa";
import "../styles/folder.css";

interface FolderListProps {
  folders: FolderResponse[];
  currentFolderId: number | null;
  setCurrentFolderId: (folderId: number | null) => void;
}

export const FolderList = ({
  folders,
  currentFolderId,
  setCurrentFolderId,
}: FolderListProps) => {
  const [isLoading, setIsLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    const fetchRootFolders = async () => {
      try {
        setIsLoading(true);
        const rootFolders = await folderService.getRootFolders();
        setFolders(rootFolders);
        setError(null);
      } catch (error) {
        console.error("Error fetching root folders:", error);
        setError("Failed to load root folders. Please try again later");
      } finally {
        setIsLoading(false);
      }
    };

    fetchRootFolders();
  }, []);

  if (isLoading) {
    return <div>Loading folders...</div>;
  }

  if (error) {
    return <div className="error-messager">{error}</div>;
  }

  if (folders.length === 0) {
    return <div>No folders found. Create a folder to get started.</div>;
  }

  return (
    <div className="folder-list">
      <h2>Folders</h2>
      <div className="folder-list-header">
        <span className="folder-name-header">Name</span>
        <span className="folder-created-header">Created</span>
      </div>
      <div className="folder-items">
        <FaRegFolder className="folder-icon" size={20} />
        {folders.map((folder) => (
          <li key={folder.id} className="folder-item">
            <span className="folder-name">{folder.name}</span>
            <span className="folder-created">
              {new Date(folder.createdDate).toLocaleDateString()}
            </span>
          </li>
        ))}
        <span className="folder-name"></span>
      </div>
    </div>
  );
};
