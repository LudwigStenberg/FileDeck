public class FileNotFoundException : Exception
{
    public int FileId { get; }

    public FileNotFoundException(int fileId) : base($"The file with ID: '{fileId}' could not be found.")
    {
        FileId = fileId;
    }
}