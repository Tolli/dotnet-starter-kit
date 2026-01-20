using System.Text;
using MediatR;

namespace FSH.Starter.WebApi.Booking.Application.Receipts.Update.v1;
public sealed record UpdateReceiptCommand(
    Guid Id,
    Guid GroupMemberId,
    decimal? AmountTotal,
    decimal? AmountPaid,
    decimal? Discount,
    DateTime? ReceiptDate,
    DateTime? DueDate,
    bool? IsPaid,
    string? ExternalReference) : IRequest<UpdateReceiptResponse>;
