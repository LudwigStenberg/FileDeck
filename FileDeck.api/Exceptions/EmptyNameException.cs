
public class EmptyNameException : Exception
{
    public EmptyNameException(string entityType) : base($"The {entityType} name cannot be empty.")
    { }

    public EmptyNameException() : base("The name cannot be empty")
    { }
}