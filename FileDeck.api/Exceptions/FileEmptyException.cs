public class FileEmptyException : Exception
{
    public FileEmptyException() : base("File is required and cannot be empty.")
    { }

    public FileEmptyException(string message) : base(message)
    { }
}