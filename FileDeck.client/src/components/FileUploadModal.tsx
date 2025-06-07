import { useState, useRef } from "react";
import * as fileService from "../services/fileService";
import { formatFileSize } from "../utilities/fileUtilities";

interface FileUploadModalProps {
  currentFolderId: number | null;
  onUploaded: () => void;
  onClose: () => void;
  isOpen: boolean;
}

export const FileUploadModal = ({
  currentFolderId,
  onUploaded,
  onClose,
  isOpen,
}: FileUploadModalProps) => {
  const [selectedFile, setSelectedFile] = useState<File | null>(null);
  const [isUploading, setIsUploading] = useState(false);
  const [uploadProgress, setUploadProgress] = useState(0);
  const [error, setError] = useState<string | null>(null);
  const fileInputRef = useRef<HTMLInputElement>(null);

  if (!isOpen) return null;

  const handleFileChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    if (e.target.files && e.target.files.length > 0) {
      setSelectedFile(e.target.files[0]);
      setError(null);
    }
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();

    if (!selectedFile) {
      setError("Please select a file to upload");
      return;
    }

    setIsUploading(true);
    setUploadProgress(0);
    setError(null);

    try {
      await fileService.uploadFile(selectedFile, currentFolderId);

      setSelectedFile(null);

      if (fileInputRef.current) {
        fileInputRef.current.value = "";
      }

      onUploaded();
      onClose();
    } catch (error) {
      console.error("Error uploading file:", error);
      setError("Failed to upload file. Please try again.");
    } finally {
      setIsUploading(false);
    }
  };

  return (
    <div className="modal-overlay">
      <div className="modal-content">
        <h2>Upload File</h2>
        {error && <div className="error-message">{error}</div>}

        <form onSubmit={handleSubmit}>
          <div className="form-group">
            <label htmlFor="fileInput">Select File:</label>
            <input
              type="file"
              id="fileInput"
              onChange={handleFileChange}
              disabled={isUploading}
              ref={fileInputRef}
            />

            {selectedFile && (
              <div className="file-info">
                <p>Selected file: {selectedFile.name}</p>
                <p>Size: {formatFileSize(selectedFile.size)}</p>
                <p>Type: {selectedFile.type || "Unknown"}</p>
              </div>
            )}

            {isUploading && (
              <div className="upload-progress">
                <progress value={uploadProgress} max="100"></progress>
                <span>{uploadProgress}%</span>
              </div>
            )}
          </div>

          <div className="modal-actions">
            <button
              type="button"
              onClick={onClose}
              disabled={isUploading}
              className="cancel-button"
            >
              Cancel
            </button>
            <button
              type="submit"
              disabled={isUploading || !selectedFile}
              className="submit-button"
            >
              {isUploading ? `Uploading... ${uploadProgress}%` : "Upload File"}
            </button>
          </div>
        </form>
      </div>
    </div>
  );
};
