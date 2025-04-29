import { useState } from "react";
import { FolderResponse } from "../types";

export const FolderList = () => {
  const [folders, setFolders] = useState<FolderResponse[]>([]);
  const [isLoading, setIsLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);
};
