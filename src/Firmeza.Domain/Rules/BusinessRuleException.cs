namespace Firmeza.Domain.Rules;

public sealed class BusinessRuleException : Exception
{
    public BusinessRuleException(string message)
        : base(message)
    {
    }

    public BusinessRuleException(string message, Exception inner)
        : base(message, inner)
    {
    }
}
