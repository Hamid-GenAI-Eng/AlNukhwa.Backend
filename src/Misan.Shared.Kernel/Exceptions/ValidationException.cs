using System.Collections.Generic;
using Misan.Shared.Kernel.Abstractions;

namespace Misan.Shared.Kernel.Exceptions;

public sealed class ValidationException : DomainException
{
    public ValidationException(IEnumerable<Error> errors)
        : base("Validation Failure", "One or more validation errors occurred.")
    {
        Errors = errors;
    }

    public IEnumerable<Error> Errors { get; }
}
