import api from "./api";
import {
  CreateFolderRequest,
  RenameFolderDto,
  FolderResponse,
  FileResponse,
} from "../types";

// Create folder
export const createFolder = async (
  folderData: CreateFolderRequest
): Promise<FolderResponse> => {
  const response = await api.post<FolderResponse>("/folder", folderData);
  return response.data;
};

// Get Folder
export const getFolderById = async (
  folderId: number
): Promise<FolderResponse> => {
  const response = await api.get<FolderResponse>(`/folder/${folderId}`);
  return response.data;
};

// Get files in folder
export const getFilesInFolder = async (
  folderId: number
): Promise<FileResponse[]> => {
  const response = await api.get<FileResponse[]>(
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
  const response = await api.delete<boolean>(`/folder/${folderId}`);

  return response.status === 204;
};
