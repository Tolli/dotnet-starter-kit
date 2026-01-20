using FSH.Framework.Core.Persistence;
using FSH.Starter.WebApi.Booking.Domain;
using FSH.Starter.WebApi.Booking.Domain.Exceptions;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace FSH.Starter.WebApi.Booking.Application.Receipts.Update.v1;
public sealed class UpdateReceiptHandler(
    ILogger<UpdateReceiptHandler> logger,
    [FromKeyedServices("booking:receipts")] IRepository<Receipt> repository)
    : IRequestHandler<UpdateReceiptCommand, UpdateReceiptResponse>
{
    public async Task<UpdateReceiptResponse> Handle(UpdateReceiptCommand request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);
        var receipt = await repository.GetByIdAsync(request.Id, cancellationToken);
        _ = receipt ?? throw new ReceiptNotFoundException(request.Id);
        var updatedReceipt = receipt.Update(request.GroupMemberId, request.AmountTotal, request.AmountPaid, request.Discount, request.ReceiptDate, request.DueDate, request.ExternalReference);
        await repository.UpdateAsync(updatedReceipt, cancellationToken);
        logger.LogInformation("receipt with id : {ReceiptId} updated.", receipt.Id);
        return new UpdateReceiptResponse(receipt.Id);
    }
}
