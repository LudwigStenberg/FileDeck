public class InvalidCharactersException : Exception
{
    public InvalidCharactersException(string entityType) : base($"The {entityType} name contains invalid characters.")
    { }

    public InvalidCharactersException() : base("The name contains invalid characters")
    { }
}