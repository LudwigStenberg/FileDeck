import { useEffect, useState } from "react";
import { FileResponse } from "../types";
import * as fileService from "../services/fileService";
import { formatFileSize } from "../utilities/fileUtilities";
import "../styles/file.css";

interface FilePreviewModalProps {
  fileId: number;
  onClose: () => void;
  isOpen: boolean;
}

export const FilePreviewModal = ({
  fileId,
  onClose,
  isOpen,
}: FilePreviewModalProps) => {
  // State for file metadata
  const [file, setFile] = useState<FileResponse | null>(null);
  // State for file content (cached)
  const [fileContent, setFileContent] = useState<Blob | null>(null);
  // For text file content
  const [textContent, setTextContent] = useState<string | null>(null);
  // For image previews
  const [previewUrl, setPreviewUrl] = useState<string | null>(null);
  // Visibility of preview
  const [isPreviewVisible, setIsPreviewVisible] = useState(false);

  const [isLoading, setIsLoading] = useState(false);
  const [isContentLoading, setIsContentLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);

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

    {
      /* To check the old value before toggle */
    }
    setIsPreviewVisible(!isPreviewVisible);

    if (isPreviewVisible) return;

    if (fileContent) return;

    try {
      setIsContentLoading(true);
      setError(null);

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

  const handleDownload = () => {
    if (!file) return;

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
      fileService.downloadFile(fileId);
    }
  };

  if (!isOpen) return null;

  return (
    <div className="modal-overlay">
      <div className="modal-content file-preview-modal">
        <h2>{file?.name || "File Preview"}</h2>

        {isLoading && <div>Loading file information...</div>}
        {error && <div className="error-message">{error}</div>}

        {file && !isLoading && (
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

            {/* Toggle button */}
            <button
              onClick={handlePreview}
              className="preview-button"
              disabled={isContentLoading}
            >
              {isPreviewVisible ? "Hide Preview" : "Show Preview"}
            </button>

            {/* Loading indicator */}
            {isContentLoading && (
              <div className="loading-indicator">Loading preview...</div>
            )}

            {/* Preview content area */}
            {isPreviewVisible && fileContent && !isContentLoading && (
              <div className="preview-content">
                {file.contentType === "text/plain" && textContent !== null && (
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
        </div>
      </div>
    </div>
  );
};
