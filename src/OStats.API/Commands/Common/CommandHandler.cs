using MassTransit;
using MediatR;
using OStats.Infrastructure;
using OStats.Infrastructure.Extensions;

namespace OStats.API.Commands.Common;

/// <summary>
/// Abstract base class for handling commands in the application.
/// Implements the <see cref="IRequestHandler{T, R}"/> interface from MediatR.
/// </summary>
/// <typeparam name="T">The type of the command.</typeparam>
/// <typeparam name="R">The type of the result.</typeparam>
public abstract class CommandHandler<T, R> : IRequestHandler<T, R> where T : IRequest<R>
{
    /// <summary>
    /// Handles the command.
    /// Must be implemented by derived classes.
    /// </summary>
    /// <param name="command">The command to handle.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the result of the command.</returns>
    public abstract Task<R> Handle(T command, CancellationToken cancellationToken);

    /// <summary>
    /// The database context.
    /// </summary>
    protected readonly Context _context;

    /// <summary>
    /// The publish endpoint for publishing domain events.
    /// </summary>
    protected readonly IPublishEndpoint _publishEndpoint;

    /// <summary>
    /// Initializes a new instance of the <see cref="CommandHandler{T, R}"/> class.
    /// </summary>
    /// <param name="context">The database context.</param>
    /// <param name="publishEndpoint">The publish endpoint.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="context"/> or <paramref name="publishEndpoint"/> is null.</exception>
    public CommandHandler(Context context, IPublishEndpoint publishEndpoint)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _publishEndpoint = publishEndpoint ?? throw new ArgumentNullException(nameof(publishEndpoint));
    }

    /// <summary>
    /// Saves changes to the database context and publishes domain events.
    /// </summary>
    /// <param name="context">The database context.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the number of state entries written to the database.</returns>
    protected async Task<int> SaveChangesAsync(CancellationToken cancellationToken)
    {
        var domainEvents = _context.ChangeTracker.GetAggregateRootDomainEvents();
        if (domainEvents is not null && domainEvents.Any())
        {
            foreach (var domainEvent in domainEvents)
            {
                // Publish domain events to Outbox, which will get published to message broker after EF Core transaction is committed.
                // ref: https://masstransit.io/documentation/configuration/middleware/outbox
                await _publishEndpoint.Publish(domainEvent, domainEvent.GetType(), cancellationToken);
            }
        }

        return await _context.SaveChangesAsync(cancellationToken);
    }
}
