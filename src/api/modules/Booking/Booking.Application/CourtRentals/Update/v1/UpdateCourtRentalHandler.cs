using FSH.Framework.Core.Persistence;
using FSH.Starter.WebApi.Booking.Domain;
using FSH.Starter.WebApi.Booking.Domain.Exceptions;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace FSH.Starter.WebApi.Booking.Application.CourtRentals.Update.v1;
public sealed class UpdateCourtRentalHandler(
    ILogger<UpdateCourtRentalHandler> logger,
    [FromKeyedServices("booking:courtrentals")] IRepository<CourtRental> repository)
    : IRequestHandler<UpdateCourtRentalCommand, UpdateCourtRentalResponse>
{
    public async Task<UpdateCourtRentalResponse> Handle(UpdateCourtRentalCommand request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);
        var courtrental = await repository.GetByIdAsync(request.Id, cancellationToken);
        _ = courtrental ?? throw new CourtRentalNotFoundException(request.Id);
        var updatedCourtRental = courtrental.Update(request.StartDate, request.StartTime, request.EndDate, !string.IsNullOrEmpty(request.Weekday) ? Enum.Parse<DayOfWeek>(request.Weekday) : null, request.Amount, request.Discount, request.Duration, request.Court, request.GroupId);
        await repository.UpdateAsync(updatedCourtRental, cancellationToken);
        logger.LogInformation("courtrental with id : {CourtRentalId} updated.", courtrental.Id);
        return new UpdateCourtRentalResponse(courtrental.Id);
    }
}
