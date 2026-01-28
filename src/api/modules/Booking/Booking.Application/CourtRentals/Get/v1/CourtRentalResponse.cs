using FSH.Starter.WebApi.Booking.Application.CourtRentalShares.Get.v1;

namespace FSH.Starter.WebApi.Booking.Application.CourtRentals.Get.v1;
public sealed record CourtRentalResponse(Guid? Id, DateTime StartDate, TimeSpan StartTime, string Description, DateTime EndDate, string Weekday, decimal Amount, decimal Discount, int Duration, int Court, IList<CourtRentalShareResponse>? CourtRentalShares, Guid? GroupId);
