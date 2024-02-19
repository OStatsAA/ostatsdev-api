namespace OStats.Domain.Common;

public record DomainOperationResult(bool Succeeded, string? ErrorType = null, string? ErrorMessage = null)
{
    public static DomainOperationResult Success => new DomainOperationResult(true);
    public static DomainOperationResult Failure(string errorMessage) => new DomainOperationResult(false, "Failure", errorMessage);
    public static DomainOperationResult NoActionTaken(string errorMessage) => new DomainOperationResult(false, "NoActionTaken", errorMessage);
    public static DomainOperationResult Unauthorized(string errorMessage) => new DomainOperationResult(false, "Unauthorized", errorMessage);
}