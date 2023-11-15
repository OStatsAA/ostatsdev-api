using System.Diagnostics.CodeAnalysis;
using FluentValidation.Results;

namespace OStats.API.Common;

public class CommandResult<T> : ICommandResult<T>
{
    public bool Success { get; init; }
    [MemberNotNullWhen(true, "Success")]
    public T? Value { get; init; }
    [MemberNotNullWhen(false, "Success")]
    public List<ValidationFailure>? ValidationFailures { get; init; }

    public CommandResult(List<ValidationFailure> validationFailures)
    {
        Success = false;
        ValidationFailures = validationFailures;
    }

    public CommandResult(ValidationFailure validationFailure)
    {
        Success = false;
        ValidationFailures = new List<ValidationFailure>()
        {
            validationFailure
        };
    }

    public CommandResult(T value)
    {
        Success = true;
        Value = value;
    }
}