using FSH.Starter.WebApi.Booking.Application.CourtRentals.Get.v1;
using FSH.Starter.WebApi.Booking.Application.Customers.Get.v1;
namespace FSH.Starter.WebApi.Booking.Application.CourtRentalShares.Get.v1;

public sealed record CourtRentalShareResponse(Guid? Id, CustomerResponse Customer, decimal AmountTotal, decimal AmountPaid, decimal Discount, string ExternalReference, bool IsPaid, DateTime PaidDate, DateTime DueDate);
