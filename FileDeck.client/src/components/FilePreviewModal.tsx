import { useEffect, useState } from "react";
import { FileResponse } from "../types";
import * as fileService from "../services/fileService";
import { formatFileSize } from "../utilities/fileUtilities";
import "../styles/file.css";

interface FilePreviewModalProps {
  fileId: number;
  onClose: () => void;
  isOpen: boolean;
  onFileDeleted?: () => void;
}

export const FilePreviewModal = ({
  fileId,
  onClose,
  isOpen,
  onFileDeleted,
}: FilePreviewModalProps) => {
  const [file, setFile] = useState<FileResponse | null>(null);
  const [fileContent, setFileContent] = useState<Blob | null>(null);
  const [textContent, setTextContent] = useState<string | null>(null);
  const [previewUrl, setPreviewUrl] = useState<string | null>(null);
  const [isPreviewVisible, setIsPreviewVisible] = useState(false);
  const [isLoading, setIsLoading] = useState(false);
  const [isContentLoading, setIsContentLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);

  const [showDeleteConfirmation, setShowDeleteConfirmation] = useState(false);
  const [isDeleting, setIsDeleting] = useState(false);
  const [deleteError, setDeleteError] = useState<string | null>(null);

  useEffect(() => {
    if (isOpen && fileId) {
      loadFileMetadata();
    }

    return () => {
      if (previewUrl) {
        URL.revokeObjectURL(previewUrl);
      }
    };
  }, [isOpen, fileId]);

  const loadFileMetadata = async () => {
    try {
      setIsLoading(true);
      const fileData = await fileService.getFileById(fileId);
      setFile(fileData);
    } catch (error) {
      console.error("Error loading file metadata:", error);
      setError("Failed to load file information.");
    } finally {
      setIsLoading(false);
    }
  };

  const handlePreview = async () => {
    if (!file) return;

    setIsPreviewVisible(!isPreviewVisible);

    // If we're hiding the preview, no need to load content
    if (isPreviewVisible) return;

    // If content is already loaded, no need to fetch again
    if (fileContent) return;

    try {
      setIsContentLoading(true);
      setError(null);

      // Load file content from server if not already cached
      if (!fileContent) {
        const blob = await fileService.previewFile(fileId);
        setFileContent(blob);

        if (file.contentType === "text/plain") {
          const text = await blob.text();
          setTextContent(text);
        } else if (file.contentType.startsWith("image/")) {
          const url = URL.createObjectURL(blob);
          setPreviewUrl(url);
        }
      }
    } catch (error) {
      console.error("Error loading file content:", error);
      setError("Failed to load file preview.");
      setIsPreviewVisible(false);
    } finally {
      setIsContentLoading(false);
    }
  };

  const handleDelete = async () => {
    if (!file) return;

    setIsDeleting(true);
    setDeleteError(null);

    try {
      const success = await fileService.deleteFile(fileId);

      if (success) {
        onClose();
        onFileDeleted?.();
      } else {
        setDeleteError("Failed to delete file. Please try again.");
      }
    } catch (error) {
      setDeleteError("Unable to delete file. Please check your connection.");
    } finally {
      setIsDeleting(false);
    }
  };

  const handleCancelDelete = () => {
    setShowDeleteConfirmation(false);
    setDeleteError(null);
  };

  const handleDownload = () => {
    if (!file) return;

    // If content is already loaded, create download from cached content
    if (fileContent) {
      const url = URL.createObjectURL(fileContent);
      const a = document.createElement("a") as HTMLAnchorElement;

      a.href = url;
      a.download = file.name;
      document.body.appendChild(a);
      a.click();
      document.body.removeChild(a);
      URL.revokeObjectURL(url);
    } else {
      // If not loaded..
      fileService.downloadFile(fileId);
    }
  };

  // Don't render anything if modal is not open
  if (!isOpen) return null;

  return (
    <div className="modal-overlay">
      <div className="modal-content file-preview-modal">
        <h2>File Preview</h2>

        {isLoading && <div>Loading file information...</div>}

        {error && <div className="error-message">{error}</div>}

        {file && !isLoading && (
          <>
            {showDeleteConfirmation ? (
              <div className="delete-confirmation">
                <h3>Confirm Deletion</h3>
                <p>
                  Are you sure you want to delete file:{" "}
                  <strong>{file.name}</strong>?
                </p>

                {deleteError && (
                  <div
                    className="error-message"
                    style={{ marginBottom: "1rem" }}
                  >
                    {deleteError}
                  </div>
                )}

                <div className="confirmation-actions">
                  <button
                    onClick={handleCancelDelete}
                    disabled={isDeleting}
                    className="cancel-button"
                  >
                    No, Cancel
                  </button>
                  <button
                    onClick={handleDelete}
                    disabled={isDeleting}
                    className="delete-button"
                  >
                    {isDeleting ? "Deleting..." : "Yes, Delete"}
                  </button>
                </div>
              </div>
            ) : (
              <div className="file-preview-container">
                <div className="file-info">
                  <p>
                    <strong>Name:</strong> {file.name}
                  </p>
                  <p>
                    <strong>Type:</strong> {file.contentType}
                  </p>
                  <p>
                    <strong>Size:</strong> {formatFileSize(file.size)}
                  </p>
                  <p>
                    <strong>Uploaded:</strong>{" "}
                    {new Date(file.uploadDate).toLocaleString()}
                  </p>
                </div>

                {/* Toggle button for showing/hiding preview */}
                <button
                  onClick={handlePreview}
                  className="preview-button"
                  disabled={isContentLoading}
                >
                  {isPreviewVisible ? "Hide Preview" : "Show Preview"}
                </button>

                {/* Loading indicator for preview content */}
                {isContentLoading && (
                  <div className="loading-indicator">Loading preview...</div>
                )}

                {/* Preview content area - shows different content based on file type */}
                {isPreviewVisible && fileContent && !isContentLoading && (
                  <div className="preview-content">
                    {file.contentType === "text/plain" &&
                      textContent !== null && (
                        <pre className="text-preview">{textContent}</pre>
                      )}

                    {file.contentType.startsWith("image/") && previewUrl && (
                      <img
                        src={previewUrl}
                        alt={file.name}
                        className="image-preview"
                      />
                    )}

                    {!file.contentType.startsWith("image/") &&
                      file.contentType !== "text/plain" && (
                        <p>Preview not available for this file type</p>
                      )}
                  </div>
                )}
              </div>
            )}
          </>
        )}

        {/* Modal action buttons - shown for both normal view and delete confirmation */}
        {!showDeleteConfirmation && (
          <div className="modal-actions">
            <button className="cancel-button" onClick={onClose}>
              Close
            </button>
            <button
              className="download-button"
              onClick={handleDownload}
              disabled={isLoading}
            >
              Download
            </button>
            {/* Delete button - triggers confirmation state rather than immediate deletion */}
            <button
              className="delete-button"
              onClick={() => setShowDeleteConfirmation(true)}
              disabled={isLoading}
            >
              Delete
            </button>
          </div>
        )}
      </div>
    </div>
  );
};
