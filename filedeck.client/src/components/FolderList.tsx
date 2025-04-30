import { useEffect, useState } from "react";
import { FolderResponse } from "../types";
import api from "../services/api";
import * as folderService from "../services/folderService";

export const FolderList = () => {
  const [folders, setFolders] = useState<FolderResponse[]>([]);
  const [isLoading, setIsLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    const fetchRootFolders = async () => {
      try {
        setIsLoading(true);
        const rootFolders = await folderService.getRootFolders();
        setFolders(rootFolders);
        setError(null);
      } catch (error) {
        console.error("Error fetching root folders:", error);
        setError("Failed to load root folders. Please try again later");
      } finally {
        setIsLoading(false);
      }
    };
  });
};
