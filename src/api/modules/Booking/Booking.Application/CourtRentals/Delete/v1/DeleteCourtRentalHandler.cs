using FSH.Framework.Core.Persistence;
using FSH.Starter.WebApi.Booking.Domain;
using FSH.Starter.WebApi.Booking.Domain.Exceptions;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace FSH.Starter.WebApi.Booking.Application.CourtRentals.Delete.v1;
public sealed class DeleteCourtRentalHandler(
    ILogger<DeleteCourtRentalHandler> logger,
    [FromKeyedServices("booking:courtrentals")] IRepository<CourtRental> repository)
    : IRequestHandler<DeleteCourtRentalCommand>
{
    public async Task Handle(DeleteCourtRentalCommand request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);
        var courtrental = await repository.GetByIdAsync(request.Id, cancellationToken);
        _ = courtrental ?? throw new CourtRentalNotFoundException(request.Id);
        await repository.DeleteAsync(courtrental, cancellationToken);
        logger.LogInformation("court rental with id : {CourtRentalId} deleted", courtrental.Id);
    }
}
