using System.ComponentModel;
using FluentValidation;
using FSH.Framework.Core.Persistence;
using FSH.Starter.WebApi.Booking.Domain;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace FSH.Starter.WebApi.Booking.Application.Receipts.Create.v1;
public sealed record CreateReceiptCommand(
    Guid GroupMemberId,
    [property: DefaultValue(0)] decimal AmountTotal,
    [property: DefaultValue(0)] decimal AmountPaid,
    [property: DefaultValue(0)] decimal Discount,
    DateTime ReceiptDate,
    DateTime DueDate,
    [property: DefaultValue(false)] bool IsPaid,
    string? ExternalReference) : IRequest<CreateReceiptResponse>;

public class CreateReceiptCommandValidator : AbstractValidator<CreateReceiptCommand>
{
    public CreateReceiptCommandValidator()
    {
        RuleFor(p => p.GroupMemberId).NotEmpty();
        RuleFor(p => p.AmountTotal).GreaterThan(0);
        RuleFor(p => p.AmountPaid).GreaterThan(0);
        RuleFor(p => p.IsPaid).NotNull();
    }
}

public sealed record CreateReceiptResponse(Guid? Id);

public sealed class CreateReceiptHandler(
    ILogger<CreateReceiptHandler> logger,
    [FromKeyedServices("booking:receipts")] IRepository<Receipt> repository)
    : IRequestHandler<CreateReceiptCommand, CreateReceiptResponse>
{
    public async Task<CreateReceiptResponse> Handle(CreateReceiptCommand request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);
        var customer = Receipt.Create(request.GroupMemberId, request.AmountTotal, request.AmountPaid, request.Discount, request.ReceiptDate, request.DueDate, request.ExternalReference);
        await repository.AddAsync(customer, cancellationToken);
        logger.LogInformation("group member created {ReceiptId}", customer.Id);
        return new CreateReceiptResponse(customer.Id);
    }
}

