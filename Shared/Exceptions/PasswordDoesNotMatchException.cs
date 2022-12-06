namespace UVGramWeb.Shared.Exceptions;

public class PasswordDoesNotMatchException : Exception
{
    public PasswordDoesNotMatchException(string message) : base(message)
    {
    }
}