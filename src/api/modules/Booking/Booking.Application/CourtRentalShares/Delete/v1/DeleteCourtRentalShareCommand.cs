using FSH.Framework.Core.Persistence;
using FSH.Starter.WebApi.Booking.Domain;
using FSH.Starter.WebApi.Booking.Domain.Exceptions;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace FSH.Starter.WebApi.Booking.Application.CourtRentalShares.Delete.v1;
public sealed record DeleteCourtRentalShareCommand(
    Guid Id) : IRequest;

public sealed class DeleteCourtRentalShareHandler(
    ILogger<DeleteCourtRentalShareHandler> logger,
    [FromKeyedServices("booking:courtrentalshares")] IRepository<CourtRentalShare> repository)
    : IRequestHandler<DeleteCourtRentalShareCommand>
{
    public async Task Handle(DeleteCourtRentalShareCommand request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);
        var CourtRentalShare = await repository.GetByIdAsync(request.Id, cancellationToken);
        _ = CourtRentalShare ?? throw new CourtRentalShareNotFoundException(request.Id);
        await repository.DeleteAsync(CourtRentalShare, cancellationToken);
        logger.LogInformation("CourtRentalShare with id : {CourtRentalShareId} deleted", CourtRentalShare.Id);
    }
}
