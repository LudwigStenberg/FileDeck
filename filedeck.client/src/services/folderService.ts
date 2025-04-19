import api from "./api";
import {
  CreateFolderDto,
  RenameFolderDto,
  FolderResponseDto,
  FileResponseDto,
} from "../types";

// Create folder
export const createFolder = async (
  folderData: CreateFolderDto
): Promise<FolderResponseDto> => {
  const response = await api.post<FolderResponseDto>("/folder", folderData);
  return response.data;
};

// Get Folder
export const getFolderById = async (
  folderId: number
): Promise<FolderResponseDto> => {
  const response = await api.get<FolderResponseDto>(`/folder/${folderId}`);
  return response.data;
};

// Get files in folder
export const getFilesInFolder = async (
  folderId: number
): Promise<FileResponseDto[]> => {
  const response = await api.get<FileResponseDto[]>(
    `/folder/${folderId}/files`
  );
  return response.data;
};

// Rename folder

export const renameFolder = async (
  folderId: number,
  renameData: RenameFolderDto
): Promise<boolean> => {
  const response = await api.put<{ message: string }>(
    `/folder/${folderId}/rename`,
    renameData
  );

  return response.status === 200;
};

// Delete folder
export const deleteFolder = async (folderId: number): Promise<boolean> => {
  const response = await api.delete(`/folder/${folderId}`);

  return response.status === 204;
};
