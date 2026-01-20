using FSH.Framework.Core.Paging;
using FSH.Starter.WebApi.Booking.Application.Customers.Get.v1;
using MediatR;

namespace FSH.Starter.WebApi.Booking.Application.Customers.Search.v1;

public class SearchCustomersCommand : PaginationFilter, IRequest<PagedList<CustomerResponse>>
{
    public Guid? GroupId { get; set; }
    public string? SearchText { get; set; }
}
