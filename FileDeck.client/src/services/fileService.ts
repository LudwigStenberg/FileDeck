import api from "./api";
import { FileResponse } from "../types";

// Upload File
export const uploadFile = async (
  file: File,
  folderId: number | null = null
): Promise<FileResponse> => {
  const formData = new FormData();
  formData.append("file", file);

  if (folderId !== null) {
    formData.append("folderId", folderId.toString());
  }

  const response = await api.post<FileResponse>("/files", formData, {
    headers: {
      "Content-Type": "multipart/form-data",
    },
  });

  return response.data;
};

// Download File
export const downloadFile = async (fileId: number): Promise<void> => {
  const response = await api.get(`/files/${fileId}/download`, {
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

// Preview File
export const previewFile = async (fileId: number): Promise<Blob> => {
  const response = await api.get(`/files/${fileId}/download`, {
    responseType: "blob",
  });

  return response.data;
};

// Get File By ID
export const getFileById = async (fileId: number): Promise<FileResponse> => {
  const response = await api.get<FileResponse>(`/files/${fileId}`);

  return response.data;
};

// Get Root Files
export const getRootFiles = async (): Promise<FileResponse[]> => {
  const response = await api.get<FileResponse[]>("/files/root");

  return response.data;
};

// Delete File
export const deleteFile = async (fileId: number): Promise<boolean> => {
  const response = await api.delete(`/files/${fileId}`);

  return response.status === 204;
};
