
public class FolderNotFoundException : Exception
{
    public int FolderId { get; }

    public FolderNotFoundException(int folderId) : base($"The folder with ID: {folderId} could not be found.")
    {
        FolderId = folderId;
    }
}