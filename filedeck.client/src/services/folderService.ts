import api from "./api";
import { CreateFolderDto, RenameFolderDto, FolderResponseDto } from "../types";

// Create folder
export const createFolder = async (
  folderData: CreateFolderDto
): Promise<FolderResponseDto> => {
  const response = await api.post<FolderResponseDto>("/folder", folderData);
  return response.data;
};

// Get Folder
// Get files in folder
// Rename folder
// Delete folder
