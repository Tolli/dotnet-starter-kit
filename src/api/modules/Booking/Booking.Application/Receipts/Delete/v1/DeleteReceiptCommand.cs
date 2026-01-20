using FSH.Framework.Core.Persistence;
using FSH.Starter.WebApi.Booking.Domain;
using FSH.Starter.WebApi.Booking.Domain.Exceptions;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace FSH.Starter.WebApi.Booking.Application.Receipts.Delete.v1;
public sealed record DeleteReceiptCommand(
    Guid Id) : IRequest;

public sealed class DeleteReceiptHandler(
    ILogger<DeleteReceiptHandler> logger,
    [FromKeyedServices("booking:receipts")] IRepository<Receipt> repository)
    : IRequestHandler<DeleteReceiptCommand>
{
    public async Task Handle(DeleteReceiptCommand request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);
        var Receipt = await repository.GetByIdAsync(request.Id, cancellationToken);
        _ = Receipt ?? throw new ReceiptNotFoundException(request.Id);
        await repository.DeleteAsync(Receipt, cancellationToken);
        logger.LogInformation("Receipt with id : {ReceiptId} deleted", Receipt.Id);
    }
}
