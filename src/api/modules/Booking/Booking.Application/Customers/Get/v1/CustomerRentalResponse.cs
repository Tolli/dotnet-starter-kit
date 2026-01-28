namespace FSH.Starter.WebApi.Booking.Application.Customers.Get.v1;
public sealed record CustomerRentalResponse(Guid? Id, DateTime StartDate, TimeSpan StartTime, string Description, DateTime EndDate, string Weekday, decimal Amount, decimal Discount, int Duration, int Court);
