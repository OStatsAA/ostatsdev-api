namespace OStats.Domain.Common;

public record DomainOperationResult(bool Succeeded, string? ErrorType = null, string? ErrorMessage = null)
{
    public static DomainOperationResult Success => new(true);

    public static DomainOperationResult Failure(string? errorMessage) => new(false, "Failure", errorMessage ?? "An error occurred.");

    public static DomainOperationResult InvalidUserRole() => new(false, "InvalidUserRole", "Invalid user role.");
    public static DomainOperationResult InvalidUserRole(string errorMessage) => new(false, "InvalidUserRole", errorMessage);
    
    public static DomainOperationResult NoActionTaken(string? errorMessage) => new(false, "NoActionTaken", errorMessage ?? "No action taken.");

    public static DomainOperationResult Unauthorized() => new(false, "Unauthorized", "Unauthorized.");
    public static DomainOperationResult Unauthorized(string? errorMessage) => new(false, "Unauthorized", errorMessage ?? "Unauthorized.");
}