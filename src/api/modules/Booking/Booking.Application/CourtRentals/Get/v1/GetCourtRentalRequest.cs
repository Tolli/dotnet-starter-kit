using MediatR;

namespace FSH.Starter.WebApi.Booking.Application.CourtRentals.Get.v1;
public class GetCourtRentalRequest : IRequest<CourtRentalResponse>
{
    public Guid Id { get; set; }
    public GetCourtRentalRequest(Guid id) => Id = id;
}
