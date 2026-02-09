using System;

namespace Misan.Shared.Kernel.Exceptions;

public abstract class DomainException : Exception
{
    protected DomainException(string title, string message)
        : base(message)
    {
        Title = title;
    }

    public string Title { get; }
}

public class NotFoundException : DomainException
{
    public NotFoundException(string message)
        : base("Not Found", message)
    {
    }
}

public class BadRequestException : DomainException
{
    public BadRequestException(string message)
        : base("Bad Request", message)
    {
    }
}
