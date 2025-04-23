export interface CreateFolderRequest {
  name: string;
  parentFolderId?: number;
}

export interface RenameFolderDto {
  newName: string;
}

export interface FolderResponseDto {
  id: number;
  name: string;
  parentFolderId?: number;
  createdDate: string;
}
