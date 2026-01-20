using FSH.Starter.WebApi.Booking.Application.Groups.Get.v1;

namespace FSH.Starter.WebApi.Booking.Application.Receipts.Get.v1;

public sealed record ReceiptResponse(Guid? Id, string Name, string? Description, decimal Amount, decimal Discount, GroupResponse? Group);
