using Ardalis.Specification;
using FSH.Starter.WebApi.Booking.Domain;

namespace FSH.Starter.WebApi.Booking.Application.Groups.Get.v1;

public class GetGroupSpecs : Specification<Group, GroupResponse>
{
    public GetGroupSpecs(Guid id)
    {
        Query
            .Where(p => p.Id == id)
            .Include(p => p.Members)
            .Include(p => p.CourtRentals);
    }
}
