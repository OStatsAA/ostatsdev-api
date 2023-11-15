using FluentValidation.Results;

namespace OStats.API.Common;

public interface ICommandResult<T>
{
    bool Success { get; init; }
    T? Value { get; init; }
    List<ValidationFailure>? ValidationFailures { get; init; }
}