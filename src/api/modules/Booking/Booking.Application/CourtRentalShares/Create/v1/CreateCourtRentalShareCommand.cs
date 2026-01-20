using System.ComponentModel;
using FluentValidation;
using FSH.Framework.Core.Persistence;
using FSH.Starter.WebApi.Booking.Domain;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace FSH.Starter.WebApi.Booking.Application.CourtRentalShares.Create.v1;
public sealed record CreateCourtRentalShareCommand(
    Guid CourtRentalId,
    Guid CustomerId,
    [property: DefaultValue(0)] decimal AmountTotal,
    [property: DefaultValue(0)] decimal AmountPaid,
    [property: DefaultValue(0)] decimal Discount,
    DateTime PaidDate,
    DateTime DueDate,
    [property: DefaultValue(false)] bool IsPaid,
    string? ExternalReference) : IRequest<CreateCourtRentalShareResponse>;

public class CreateCourtRentalShareCommandValidator : AbstractValidator<CreateCourtRentalShareCommand>
{
    public CreateCourtRentalShareCommandValidator()
    {
        RuleFor(p => p.CourtRentalId).NotEmpty();
        RuleFor(p => p.CustomerId).NotEmpty();
    }
}

public sealed record CreateCourtRentalShareResponse(Guid? Id);

public sealed class CreateCourtRentalShareHandler(
    ILogger<CreateCourtRentalShareHandler> logger,
    [FromKeyedServices("booking:courtrentalshares")] IRepository<CourtRentalShare> repository)
    : IRequestHandler<CreateCourtRentalShareCommand, CreateCourtRentalShareResponse>
{
    public async Task<CreateCourtRentalShareResponse> Handle(CreateCourtRentalShareCommand request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);
        var customer = CourtRentalShare.Create(request.CourtRentalId, request.CustomerId, request.AmountTotal, request.AmountPaid, request.Discount, request.PaidDate, request.DueDate, request.ExternalReference);
        await repository.AddAsync(customer, cancellationToken);
        logger.LogInformation("group member created {CourtRentalShareId}", customer.Id);
        return new CreateCourtRentalShareResponse(customer.Id);
    }
}

