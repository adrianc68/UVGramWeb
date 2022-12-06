namespace UVGramWeb.Shared.Exceptions;

public class InteralServerErrorException : Exception
{
    public InteralServerErrorException(string message) : base(message)
    {
    }

    public InteralServerErrorException(string message, Exception error) : base(message, error)
    {
    }

}