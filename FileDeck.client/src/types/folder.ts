export interface CreateFolderRequest {
  name: string;
  parentFolderId: number | null;
}

export interface RenameFolderDto {
  newName: string;
}

export interface FolderResponse {
  id: number;
  name: string;
  parentFolderId: number | null;
  createdDate: string;
}
