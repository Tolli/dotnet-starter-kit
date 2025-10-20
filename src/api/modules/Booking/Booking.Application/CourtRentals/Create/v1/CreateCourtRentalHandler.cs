using FSH.Framework.Core.Persistence;
using FSH.Starter.WebApi.Booking.Domain;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace FSH.Starter.WebApi.Booking.Application.CourtRentals.Create.v1;
public sealed class CreateCourtRentalHandler(
    ILogger<CreateCourtRentalHandler> logger,
    [FromKeyedServices("booking:courtrentals")] IRepository<CourtRental> repository)
    : IRequestHandler<CreateCourtRentalCommand, CreateCourtRentalResponse>
{
    public async Task<CreateCourtRentalResponse> Handle(CreateCourtRentalCommand request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);
        var courtrental = CourtRental.Create(request.Name!, request.Description, request.Price, request.GroupId);
        await repository.AddAsync(courtrental, cancellationToken);
        logger.LogInformation("courtrental created {CourtRentalId}", courtrental.Id);
        return new CreateCourtRentalResponse(courtrental.Id);
    }
}
