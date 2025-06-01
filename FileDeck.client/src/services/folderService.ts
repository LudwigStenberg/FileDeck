import api from "./api";
import {
  CreateFolderRequest,
  Renamerequest,
  FolderResponse,
  FileResponse,
} from "../types";

// Create folder
export const createFolder = async (
  folderData: CreateFolderRequest
): Promise<FolderResponse> => {
  const response = await api.post<FolderResponse>("/folders", folderData);
  return response.data;
};

// Get folder
export const getFolderById = async (
  folderId: number
): Promise<FolderResponse> => {
  const response = await api.get<FolderResponse>(`/folders/${folderId}`);
  return response.data;
};

// Get all folders
export const getAllFolders = async (): Promise<FolderResponse[]> => {
  const response = await api.get<FolderResponse[]>("/folders/all");
  return response.data;
};

// Get root folders
export const getRootFolders = async (): Promise<FolderResponse[]> => {
  const response = await api.get<FolderResponse[]>("/folders/root");
  return response.data;
};

// Get files in folder
export const getFilesInFolder = async (
  folderId: number
): Promise<FileResponse[]> => {
  const response = await api.get<FileResponse[]>(`/folders/${folderId}/files`);
  return response.data;
};

// Rename folder
export const renameFolder = async (
  folderId: number,
  renameData: Renamerequest
): Promise<boolean> => {
  const response = await api.put<{ message: string }>(
    `/folders/${folderId}/rename`,
    renameData
  );

  return response.status === 200;
};

// Delete folder
export const deleteFolder = async (folderId: number): Promise<boolean> => {
  const response = await api.delete<boolean>(`/folders/${folderId}`);

  return response.status === 204;
};
