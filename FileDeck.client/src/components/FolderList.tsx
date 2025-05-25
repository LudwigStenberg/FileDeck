import { useState } from "react";
import { FolderResponse } from "../types";
import { FaRegFolder } from "react-icons/fa";
import { IoArrowBack } from "react-icons/io5";
import { GoHome } from "react-icons/go";
import { RiDeleteBin6Line } from "react-icons/ri";
import { Breadcrumb } from "./Breadcrumb";
import "../styles/modal.css";
import "../styles/folder.css";
import * as folderService from "../services/folderService";

interface FolderListProps {
  folders: FolderResponse[];
  currentFolderId: number | null;
  setCurrentFolderId: (folderId: number | null) => void;
  onFolderDeleted?: () => void;
}

export const FolderList = ({
  folders,
  currentFolderId,
  setCurrentFolderId,
  onFolderDeleted,
}: FolderListProps) => {
  const [folderToDelete, setFolderToDelete] = useState<number | null>(null);
  const [isDeleting, setIsDeleting] = useState(false);
  const [deleteError, setDeleteError] = useState<string | null>(null);

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

  const handleDeleteClick = (e: React.MouseEvent, folderId: number) => {
    e.stopPropagation();
    setFolderToDelete(folderId);
  };

  const handleConfirmDelete = async () => {
    if (!folderToDelete) return;

    setIsDeleting(true);
    setDeleteError(null);

    try {
      const success = await folderService.deleteFolder(folderToDelete);

      if (success) {
        if (folderToDelete === currentFolderId) {
          const deletedFolder = folders.find((f) => f.id === folderToDelete);
          setCurrentFolderId(deletedFolder?.parentFolderId ?? null);
        }

        setFolderToDelete(null);

        onFolderDeleted?.();
      } else {
        setDeleteError("Failed to delete folder. Please try again.");
      }
    } catch (error) {
      setDeleteError("Unable to delete folder. Please check your connection.");
    } finally {
      setIsDeleting(false);
    }
  };

  const handleCancelDelete = () => {
    setFolderToDelete(null);
    setDeleteError(null);
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
                <FaRegFolder className="folder-icon" size={25} />
                <span className="folder-name">{folder.name}</span>
                <RiDeleteBin6Line
                  className="delete-icon"
                  size={22}
                  onClick={(e) => handleDeleteClick(e, folder.id)}
                />
              </li>
            ))}
          </>
        ) : (
          <div className="empty-folder-message"></div>
        )}
      </div>

      {/* Delete Confirmation Modal */}
      {folderToDelete && (
        <div className="modal-overlay">
          <div className="modal-content">
            <h3>Confirm Deletion</h3>
            <p>
              Are you sure you want to delete this folder and all its contents?
              This action cannot be undone.
            </p>

            {deleteError && (
              <div className="error-message" style={{ marginBottom: "1rem" }}>
                {deleteError}
              </div>
            )}

            <div className="modal-actions">
              <button
                onClick={handleCancelDelete}
                disabled={isDeleting}
                className="cancel-button"
              >
                Cancel
              </button>
              <button
                onClick={handleConfirmDelete}
                disabled={isDeleting}
                className="delete-button"
              >
                {isDeleting ? "Deleting..." : "Delete"}
              </button>
            </div>
          </div>
        </div>
      )}
    </div>
  );
};
