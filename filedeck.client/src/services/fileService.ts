import api from "./api";
import {
  FileUploadRequest,
  FileDownloadResponse,
  FileResponse,
} from "../types";

export const uploadFile = async (
  fileData: FileUploadRequest
): Promise<FileResponse> => {
  const response = await api.post<FileResponse>("/file", fileData);

  return response.data;
};

export const getFileById = async (fileId: number): Promise<FileResponse> => {
  const response = await api.get<FileResponse>(`/file/${fileId}`);

  return response.data;
};

export const downloadFile = async (fileId: number): Promise<void> => {
  const response = await api.get(`/file/${fileId}/download`, {
    responseType: "blob",
  });

  const url = window.URL.createObjectURL(new Blob([response.data]));

  const link = document.createElement("a");
  link.href = url;

  const contentDisposition = response.headers["content-disposition"];
  const fileName = contentDisposition
    ? contentDisposition.split("filename=")[1].replace(/"/g, "")
    : `file-${fileId}`;

  link.setAttribute("download", fileName);

  document.body.appendChild(link);
  link.click();
  document.body.removeChild(link);
};

export const deleteFile = async (fileId: number): Promise<boolean> => {
  const response = await api.delete(`/file/${fileId}`);

  return response.status === 204;
};
