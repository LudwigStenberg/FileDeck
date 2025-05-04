import { useState } from "react";
import { FolderResponse } from "../types";
import { FaRegFolder } from "react-icons/fa";
import { IoArrowBack } from "react-icons/io5";
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
  const foldersToDisplay = folders.filter(
    (folder) =>
      currentFolderId === null
        ? folder.parentFolderId === null // Show root folders when at root
        : folder.parentFolderId === currentFolderId // Show subfolders when in a folder
  );

  const handleFolderClick = (folderId: number) => {
    setCurrentFolderId(folderId);
  };

  const handleGoUp = () => {
    if (currentFolderId === null) return;

    const currentFolder = folders.find((f) => f.id === currentFolderId);
    setCurrentFolderId(currentFolder?.parentFolderId ?? null);
  };

  if (folders.length === 0) {
    return <div>No folders found. Create a folder to get started.</div>;
  }

  return (
    <div className="folder-list">
      <h2>Folders</h2>

      <div className="navigation-container">
        <IoArrowBack className="go-up-icon" size={25} />
        <span className="navigation-path">Path</span>
      </div>

      <div className="folder-list-header">
        <span></span>
        <span className="folder-name-header">Name</span>
        <span className="folder-created-header">Created</span>
      </div>
      <div className="folder-items">
        <FaRegFolder className="folder-icon" size={20} />
        {foldersToDisplay.map((folder) => (
          <li
            key={folder.id}
            className="folder-item"
            onClick={() => handleFolderClick(folder.id)}
          >
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
