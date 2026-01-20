using Ardalis.Specification;
using FSH.Framework.Core.Paging;
using FSH.Framework.Core.Specifications;
using FSH.Starter.WebApi.Booking.Application.Customers.Get.v1;
using FSH.Starter.WebApi.Booking.Domain;

namespace FSH.Starter.WebApi.Booking.Application.Customers.Search.v1;
public class SearchCustomerSpecs : EntitiesByPaginationFilterSpec<Customer, CustomerResponse>
{
    public SearchCustomerSpecs(SearchCustomersCommand command)
        : base(command) =>
        Query
            .OrderBy(c => c.Name, !command.HasOrderBy());
}
