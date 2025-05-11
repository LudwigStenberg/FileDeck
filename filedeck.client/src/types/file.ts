export interface FileUploadRequest {
  name: string;
  contentType: string;
  content: string; // base64
  folderId?: number | null;
}

export interface FileDownloadResponse {
  id: number;
  name: string;
  contentType: string;
  content: Uint8Array | ArrayBuffer;
}

export interface FileResponse {
  id: number;
  name: string;
  contentType: string;
  size: number;
  uploadDate: string;
  lastModifiedDate: string;
  folderId?: number;
}
