export interface CreateFolderRequest {
  name: string;
  parentFolderId?: number;
}

export interface RenameFolderDto {
  newName: string;
}

export interface FolderResponse {
  id: number;
  name: string;
  parentFolderId?: number;
  createdDate: string;
}
