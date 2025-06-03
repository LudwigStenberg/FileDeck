
public class UserNotFoundException : Exception
{
    public string UserId { get; }
    public UserNotFoundException(string userId) : base($"The user with ID: '{userId}' could not be found.")
    {
        UserId = userId;
    }
}