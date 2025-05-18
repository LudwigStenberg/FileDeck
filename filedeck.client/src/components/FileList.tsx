import { useState } from "react";
import { FileResponse } from "../types";
import { FaRegFileAlt } from "react-icons/fa";
import { FilePreviewModal } from "./FilePreviewModal";

interface FileListProps {
  files: FileResponse[];
  onFileDeleted: () => void;
}

export const FileList = ({ files, onFileDeleted }: FileListProps) => {
  const [selectedFileId, setSelectedFileId] = useState<number | null>(null);
  const [isPreviewModalOpen, setIsPreviewModalOpen] = useState(false);

  const handleFileClick = (fileId: number) => {
    setSelectedFileId(fileId);
    setIsPreviewModalOpen(true);
  };

  if (files.length === 0) {
    return "";
  }

  return (
    <div className="file-list">
      <ul className="file-items">
        <FaRegFileAlt className="file-icon" size={22} />
        {files.map((file) => (
          <li
            key={file.id}
            className="file-item"
            onClick={() => handleFileClick(file.id)}
          >
            <span className="file-name">{file.name}</span>
            <span className="file-actions"></span>
          </li>
        ))}
      </ul>

      {selectedFileId && (
        <FilePreviewModal
          fileId={selectedFileId}
          isOpen={isPreviewModalOpen}
          onClose={() => setIsPreviewModalOpen(false)}
        />
      )}
    </div>
  );
};
