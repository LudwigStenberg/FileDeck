export interface FileUploadDto {
  name: string;
  contentType: string;
  content: Uint8Array | ArrayBuffer;
  folderId?: number;
}

export interface FileDownloadDto {
  id: number;
  name: string;
  contentType: string;
  content: Uint8Array | ArrayBuffer;
}

export interface FileResponseDto {
  id: number;
  name: string;
  contentType: string;
  size: number;
  uploadDate: string;
  lastModifiedDate: string;
  folderId?: number;
}
