namespace Firmeza.Application.Errors;

public sealed class NotFoundException : Exception
{
    public NotFoundException(string message)
        : base(message)
    {
    }
}

public sealed class InvalidCredentialsException : Exception
{
    public InvalidCredentialsException(string message)
        : base(message)
    {
    }
}

public sealed class AccessDeniedException : Exception
{
    public AccessDeniedException(string message)
        : base(message)
    {
    }
}

public sealed class ConflictException : Exception
{
    public ConflictException(string message)
        : base(message)
    {
    }
}
