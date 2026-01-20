using FSH.Framework.Core.Persistence;
using FSH.Starter.WebApi.Booking.Domain;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace FSH.Starter.WebApi.Booking.Application.CourtRentalSessions.Create.v1;
public sealed class CreateCourtRentalSessionHandler(
    ILogger<CreateCourtRentalSessionHandler> logger,
    [FromKeyedServices("booking:courtrentalsessions")] IRepository<CourtRentalSession> repository)
    : IRequestHandler<CreateCourtRentalSessionCommand, CreateCourtRentalSessionResponse>
{
    public async Task<CreateCourtRentalSessionResponse> Handle(CreateCourtRentalSessionCommand request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);
        var courtRentalSession = CourtRentalSession.Create(request.StartDate, request.EndDate, request.Court, request.CourtRentalId);
        await repository.AddAsync(courtRentalSession, cancellationToken);
        logger.LogInformation("courtrentalSession created {CourtRentalSessionId}", courtRentalSession.Id);
        return new CreateCourtRentalSessionResponse(courtRentalSession.Id);
    }
}
