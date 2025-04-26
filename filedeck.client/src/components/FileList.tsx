import { useEffect, useState } from "react";
import api from "../services/api";
import * as fileService from "../services/fileService";
import { FileResponse } from "../types";

export const FileList = () => {
  const [files, setFiles] = useState<FileResponse[]>();
  const [isLoading, setIsLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    const fetchFiles = async () => {
      try {
        setIsLoading(true);
        const rootFiles = await fileService.getRootFiles();
        setFiles(rootFiles);
        setError(null);
      } catch (error) {
        console.error("Error fetching files:", error);
        setError("Failed to load files. Please try again later");
      } finally {
        setIsLoading(false);
      }
    };

    fetchFiles();
  }, []);
};
