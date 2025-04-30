import { useEffect, useState } from "react";
import api from "../services/api";
import * as fileService from "../services/fileService";
import { FileResponse } from "../types";
import "../styles/file.css";

export const FileList = () => {
  const [files, setFiles] = useState<FileResponse[]>([]);
  const [isLoading, setIsLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    const fetchFiles = async () => {
      try {
        setIsLoading(true);
        const rootFiles = await fileService.getRootFiles();
        setFiles(rootFiles);
        setError(null);
      } catch (error) {
        console.error("Error fetching files:", error);
        setError("Failed to load files. Please try again later");
      } finally {
        setIsLoading(false);
      }
    };

    fetchFiles();
  }, []);

  if (isLoading) {
    return <div>Loading files...</div>;
  }

  if (error) {
    return <div className="error-message">{error}</div>;
  }

  if (files.length === 0) {
    return (
      <div className="no-files-found">
        No files found. Upload some files to get started.
      </div>
    );
  }

  return (
    <div className="file-list">
      <h2>Your Files</h2>
      <div className="file-list-header">
        <span className="file-name">Name</span>
        <span className="file-type">Type</span>
        <span className="file-size">Size</span>
        <span className="file-date">Modified</span>
      </div>
      <ul className="file-items">
        {files.map((file) => (
          <li key={file.id} className="file-item">
            <span className="file-name">{file.name}</span>
            <span className="file-type">{file.contentType}</span>
            <span className="file-size">{file.size}</span>
            <span className="file-date">
              {new Date(file.lastModifiedDate).toLocaleDateString()}
            </span>
          </li>
        ))}
      </ul>
    </div>
  );
};
