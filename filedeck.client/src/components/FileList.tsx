import { useEffect, useState } from "react";
import { FileResponse } from "../types";
import "../styles/file.css";

interface FileListProps {
  files: FileResponse[];
  onFileDeleted: () => void;
}

export const FileList = ({ files, onFileDeleted }: FileListProps) => {
  if (files.length === 0) {
    return "";
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
            <span className="file-actions"></span>
            {/* TODO: Add download/delete buttons */}
          </li>
        ))}
      </ul>
    </div>
  );
};
