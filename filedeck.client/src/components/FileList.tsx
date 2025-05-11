import { useEffect, useState } from "react";
import { FileResponse } from "../types";
import { FaRegFileAlt } from "react-icons/fa";
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
      <ul className="file-items">
        <FaRegFileAlt className="file-icon" />
        {files.map((file) => (
          <li key={file.id} className="file-item">
            <span className="file-name">{file.name}</span>
            <span className="file-actions"></span>
            {/* TODO: Add download/delete buttons */}
          </li>
        ))}
      </ul>
    </div>
  );
};
