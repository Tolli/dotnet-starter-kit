namespace FSH.Starter.WebApi.Booking.Application.Groups.Get.v1;
public sealed record GroupResponse(Guid? Id, string Name, DateTime StartDate, DateTime EndDate, Guid? ContactId);
