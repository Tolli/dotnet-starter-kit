using FluentValidation;
using FSH.Framework.Core.Persistence;
using FSH.Starter.WebApi.Booking.Domain;
using FSH.Starter.WebApi.Booking.Domain.Exceptions;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace FSH.Starter.WebApi.Booking.Application.CourtRentalShares.Update.v1;

public class UpdateCourtRentalShareCommandValidator : AbstractValidator<UpdateCourtRentalShareCommand>
{
    public UpdateCourtRentalShareCommandValidator()
    {
        RuleFor(p => p.Id).NotEmpty();
        RuleFor(p => p.CourtRentalId).NotEmpty();
    }
}

public sealed record UpdateCourtRentalShareResponse(Guid? Id);
public sealed record UpdateCourtRentalShareCommand(
    Guid Id,
    Guid CourtRentalId,
    Guid CustomerId,
    decimal? AmountTotal,
    decimal? AmountPaid,
    decimal? Discount,
    DateTime? PaidDate,
    DateTime? DueDate,
    bool? IsPaid,
    string? ExternalReference) : IRequest<UpdateCourtRentalShareResponse>;
public sealed class UpdateCourtRentalShareHandler(
    ILogger<UpdateCourtRentalShareHandler> logger,
    [FromKeyedServices("booking:courtrentalshares")] IRepository<CourtRentalShare> repository)
    : IRequestHandler<UpdateCourtRentalShareCommand, UpdateCourtRentalShareResponse>
{
    public async Task<UpdateCourtRentalShareResponse> Handle(UpdateCourtRentalShareCommand request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);
        var courtrentalshare = await repository.GetByIdAsync(request.Id, cancellationToken);
        _ = courtrentalshare ?? throw new CourtRentalShareNotFoundException(request.Id);
        var updatedCourtRentalShare = courtrentalshare.Update(request.CourtRentalId, request.CustomerId, request.AmountTotal, request.AmountPaid, request.Discount, request.PaidDate, request.DueDate, request.ExternalReference);
        await repository.UpdateAsync(updatedCourtRentalShare, cancellationToken);
        logger.LogInformation("courtrentalshare with id : {CourtRentalShareId} updated.", courtrentalshare.Id);
        return new UpdateCourtRentalShareResponse(courtrentalshare.Id);
    }
}
