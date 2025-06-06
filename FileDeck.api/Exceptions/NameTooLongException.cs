
public class NameTooLongException : Exception
{
    public NameTooLongException(string entityType, int maxLength)
        : base($"The {entityType} name cannot be longer than {maxLength} characters.")
    { }

    public NameTooLongException() : base("The name is too long.")
    { }
}