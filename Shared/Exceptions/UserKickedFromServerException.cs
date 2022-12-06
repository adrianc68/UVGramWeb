namespace UVGramWeb.Shared.Exceptions;

public class UserKickedFromServerException : Exception
{
    public UserKickedFromServerException(string message) : base(message)
    {
    }
}