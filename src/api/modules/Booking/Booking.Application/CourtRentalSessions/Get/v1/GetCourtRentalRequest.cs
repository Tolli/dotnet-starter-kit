using MediatR;

namespace FSH.Starter.WebApi.Booking.Application.CourtRentalSessions.Get.v1;
public class GetCourtRentalSessionRequest : IRequest<CourtRentalSessionResponse>
{
    public Guid Id { get; set; }
    public GetCourtRentalSessionRequest(Guid id) => Id = id;
}
