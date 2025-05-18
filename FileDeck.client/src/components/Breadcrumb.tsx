import { FolderResponse } from "../types";

interface BreadcrumbProps {
  folders: FolderResponse[];
  currentFolderId: number | null;
  setCurrentFolderId: (folderId: number | null) => void;
}

export const Breadcrumb = ({
  folders,
  currentFolderId,
  setCurrentFolderId,
}: BreadcrumbProps) => {
  const buildPath = (): FolderResponse[] => {
    if (currentFolderId === null) return [];

    const path: FolderResponse[] = [];
    let currentId: number | null = currentFolderId;

    while (currentId !== null) {
      const folder = folders.find((f) => f.id === currentId);
      if (!folder) break;

      path.unshift(folder);
      currentId = folder.parentFolderId;
    }
    return path;
  };

  const path = buildPath();

  return (
    <div className="breadcrumb-container">
      <span
        className="breadcrumb-item"
        onClick={() => setCurrentFolderId(null)}
      >
        Home
      </span>

      {path.map((folder) => (
        <span key={folder.id}>
          <span
            className="breadcrumb-item"
            onClick={() => setCurrentFolderId(folder.id)}
          >
            /{folder.name}
          </span>
        </span>
      ))}
    </div>
  );
};
