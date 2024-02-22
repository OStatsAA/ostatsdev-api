using System.ComponentModel.DataAnnotations;

namespace OStats.API.Filters;

public class ValidationFilter<T> : IEndpointFilter
{
    public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
    {
        var model = context.Arguments.First(arg => arg is T);
        if (model is null)
        {
            return TypedResults.BadRequest("Model is null");
        }

        var validationContext = new ValidationContext(model);
        var validationResults = new List<ValidationResult>();
        bool isValid = Validator.TryValidateObject(model, validationContext, validationResults, true);

        if (!isValid)
        {
            return TypedResults.BadRequest(string.Join(",", validationResults.Select(x => x.ErrorMessage)));
        }

        return await next(context);
    }
}
