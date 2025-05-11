import { useState } from "react";
import { CreateFolderRequest } from "../types";
import * as folderService from "../services/folderService";

interface CreateFolderModalProps {
  currentFolderId: number | null;
  onFolderCreated: () => void;
  onClose: () => void;
  isOpen: boolean;
}

export const CreateFolderModal = ({
  currentFolderId,
  onFolderCreated,
  onClose,
  isOpen,
}: CreateFolderModalProps) => {
  const [folderName, setFolderName] = useState("");
  const [isLoading, setIsLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);

  if (!isOpen) return null;

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();

    if (!folderName.trim()) {
      setError("Folder name cannot be empty");
      return;
    }

    setIsLoading(true);
    setError(null);

    try {
      const folderData: CreateFolderRequest = {
        name: folderName,
        parentFolderId: currentFolderId,
      };

      await folderService.createFolder(folderData);
      setFolderName("");
      onFolderCreated();
      onClose();
    } catch (error) {
      console.error("Error creating folder:", error);
      setError("Failed to create folder. Please try again.");
    } finally {
      setIsLoading(false);
    }
  };

  return (
    <div className="modal-overlay">
      <div className="modal-content">
        <h2>Create New Folder</h2>
        {error && <div className="error-message">{error}</div>}

        <form onSubmit={handleSubmit}>
          <div className="form-group">
            <label htmlFor="folderName">Folder Name:</label>
            <input
              type="text"
              id="folderName"
              value={folderName}
              onChange={(e) => setFolderName(e.target.value)}
              disabled={isLoading}
              placeholder="Enter folder name"
              autoFocus
            />
          </div>

          <div className="modal-actions">
            <button
              className="cancel-button"
              type="button"
              onClick={onClose}
              disabled={isLoading}
            >
              Cancel
            </button>
            <button
              className="submit-button"
              type="submit"
              disabled={isLoading}
            >
              {isLoading ? "Creating..." : "Create Folder"}
            </button>
          </div>
        </form>
      </div>
    </div>
  );
};
