import { useState } from "react";
import { FolderResponse } from "../types";
import { FaRegFolder } from "react-icons/fa";
import { IoArrowBack } from "react-icons/io5";
import "../styles/folder.css";
import { Breadcrumb } from "./Breadcrumb";

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
  const displayedFolders = folders.filter(
    (folder) =>
      currentFolderId === null
        ? folder.parentFolderId === null // Show root folders when at root
        : folder.parentFolderId === currentFolderId // Show subfolders when in a folder
  );

  // Navigate to folder on click
  const handleFolderClick = (folderId: number) => {
    setCurrentFolderId(folderId);
  };

  // Navigate to parent folder
  const handleGoUp = () => {
    if (currentFolderId === null) return;

    const currentFolder = folders.find((f) => f.id === currentFolderId);
    setCurrentFolderId(currentFolder?.parentFolderId ?? null);
  };

  return (
    <div className="folder-list">
      <h2>Folders</h2>
      <div className="navigation-container">
        <IoArrowBack className="go-up-icon" size={25} onClick={handleGoUp} />
        <span className="navigation-path">
          {" "}
          <Breadcrumb
            folders={folders}
            currentFolderId={currentFolderId as number | null}
            setCurrentFolderId={setCurrentFolderId}
          />
        </span>
      </div>

      <div className="folder-list-header">
        <span></span>
        <span className="folder-name-header">Name</span>
        <span className="folder-created-header">Created</span>
      </div>

      <div className="folder-items">
        {displayedFolders.length > 0 ? (
          <>
            <FaRegFolder className="folder-icon" size={20} />
            {displayedFolders.map((folder) => (
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
          </>
        ) : (
          <div className="empty-folder-message">
            This folder is empty. Create a new folder or upload files.
          </div>
        )}
      </div>
    </div>
  );
};
