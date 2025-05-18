import { useState } from "react";
import { FolderResponse } from "../types";
import { FaRegFolder } from "react-icons/fa";
import { IoArrowBack } from "react-icons/io5";
import { GoHome } from "react-icons/go";

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
      <div className="navigation-container">
        <IoArrowBack className="go-up-icon" size={25} onClick={handleGoUp} />
        <GoHome
          className="go-home-icon"
          size={23}
          onClick={() => setCurrentFolderId(null)}
        />
        <span className="navigation-path">
          <Breadcrumb
            folders={folders}
            currentFolderId={currentFolderId as number | null}
            setCurrentFolderId={setCurrentFolderId}
          />
        </span>
      </div>

      <div className="folder-list-header">
        <span></span>
      </div>

      <div className="folder-items">
        {displayedFolders.length > 0 ? (
          <>
            {displayedFolders.map((folder) => (
              <li
                key={folder.id}
                className="folder-item"
                onClick={() => handleFolderClick(folder.id)}
              >
                <FaRegFolder className="folder-icon" size={20} />
                <span className="folder-name">{folder.name}</span>
              </li>
            ))}
          </>
        ) : (
          <div className="empty-folder-message"></div>
        )}
      </div>
    </div>
  );
};
