using Ardalis.Specification;
using FSH.Starter.WebApi.Booking.Domain;

namespace FSH.Starter.WebApi.Booking.Application.Customers.Get.v1;

public class GetCustomerSpecs : Specification<Customer, CustomerResponse>
{
    public GetCustomerSpecs(Guid id)
    {
        Query
            .Include(c => c.CourtRentalShares)
            .Where(p => p.Id == id);
    }
}
