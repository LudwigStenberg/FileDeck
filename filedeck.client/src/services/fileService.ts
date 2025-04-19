import api from "./api";
import { FileUploadDto, FileDownloadDto, FileResponseDto } from "../types";

export const uploadFile = async (
  fileData: FileUploadDto
): Promise<FileResponseDto> => {
  const response = await api.post<FileResponseDto>("/file", fileData);

  return response.data;
};

export const getFileById = async (fileId: number): Promise<FileResponseDto> => {
  const response = await api.get<FileResponseDto>(`/file/${fileId}`);

  return response.data;
};
