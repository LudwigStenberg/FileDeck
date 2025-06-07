public class FileTooLargeException : Exception
{
    public FileTooLargeException() : base("The file size cannot exceed 50MB.")
    { }
}