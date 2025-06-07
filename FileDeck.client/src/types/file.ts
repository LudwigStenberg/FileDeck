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
