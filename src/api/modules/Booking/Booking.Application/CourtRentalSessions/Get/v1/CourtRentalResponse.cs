using FSH.Starter.WebApi.Booking.Application.CourtRentals.Get.v1;

namespace FSH.Starter.WebApi.Booking.Application.CourtRentalSessions.Get.v1;
public sealed record CourtRentalSessionResponse(Guid? Id, DateTime StartDate, DateTime EndDate, int Court, CourtRentalResponse CourtRental);
