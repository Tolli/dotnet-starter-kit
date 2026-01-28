namespace FSH.Starter.WebApi.Booking.Application.CourtRentalShares.Get.v1;
public sealed record ShareCourtRentalResponsed(Guid? Id, DateTime StartDate, TimeSpan StartTime, string Description, DateTime EndDate, string Weekday, decimal Amount, decimal Discount, int Duration, int Court, Guid? GroupId);
