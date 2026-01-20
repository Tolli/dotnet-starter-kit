using FSH.Framework.Core.Persistence;
using FSH.Starter.WebApi.Booking.Application.CourtRentals.Update.v1;
using FSH.Starter.WebApi.Booking.Domain;
using FSH.Starter.WebApi.Booking.Domain.Exceptions;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace FSH.Starter.WebApi.Booking.Application.CourtRentalSessions.Update.v1;
public sealed class UpdateCourtRentalSessionHandler(
    ILogger<UpdateCourtRentalSessionHandler> logger,
    [FromKeyedServices("booking:courtrentalsessions")] IRepository<CourtRentalSession> repository)
    : IRequestHandler<UpdateCourtRentalSessionCommand, UpdateCourtRentalSessionResponse>
{
    public async Task<UpdateCourtRentalSessionResponse> Handle(UpdateCourtRentalSessionCommand request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);
        var courtrentalSession = await repository.GetByIdAsync(request.Id, cancellationToken);
        _ = courtrentalSession ?? throw new CourtRentalNotFoundException(request.Id);
        var updatedCourtRentalSession = courtrentalSession.Update(request.StartDate, request.EndDate, request.Court, request.CourtRentalId);
        await repository.UpdateAsync(updatedCourtRentalSession, cancellationToken);
        logger.LogInformation("courtrentalsession with id : {courtRentalSessionId} updated.", courtrentalSession.Id);
        return new UpdateCourtRentalSessionResponse(courtrentalSession.Id);
    }
}
