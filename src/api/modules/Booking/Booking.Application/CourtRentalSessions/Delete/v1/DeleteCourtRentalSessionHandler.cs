using FSH.Framework.Core.Persistence;
using FSH.Starter.WebApi.Booking.Domain;
using FSH.Starter.WebApi.Booking.Domain.Exceptions;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace FSH.Starter.WebApi.Booking.Application.CourtRentalSessions.Delete.v1;
public sealed class DeleteCourtRentalHandler(
    ILogger<DeleteCourtRentalHandler> logger,
    [FromKeyedServices("booking:courtrentalsessions")] IRepository<CourtRentalSession> repository)
    : IRequestHandler<DeleteCourtRentalSessionCommand>
{
    public async Task Handle(DeleteCourtRentalSessionCommand request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);
        var courtrentalsession = await repository.GetByIdAsync(request.Id, cancellationToken);
        _ = courtrentalsession ?? throw new CourtRentalSessionNotFoundException(request.Id);
        await repository.DeleteAsync(courtrentalsession, cancellationToken);
        logger.LogInformation("court rental session with id : {CourtRentalSessionId} deleted", courtrentalsession.Id);
    }
}
