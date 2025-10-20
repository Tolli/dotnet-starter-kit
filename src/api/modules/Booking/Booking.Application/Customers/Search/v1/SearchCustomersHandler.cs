using FSH.Framework.Core.Paging;
using FSH.Framework.Core.Persistence;
using FSH.Starter.WebApi.Booking.Application.Customers.Get.v1;
using FSH.Starter.WebApi.Booking.Domain;
using MediatR;
using Microsoft.Extensions.DependencyInjection;


namespace FSH.Starter.WebApi.Booking.Application.Customers.Search.v1;
public sealed class SearchCustomersHandler(
    [FromKeyedServices("booking:customers")] IReadRepository<Customer> repository)
    : IRequestHandler<SearchCustomersCommand, PagedList<CustomerResponse>>
{
    public async Task<PagedList<CustomerResponse>> Handle(SearchCustomersCommand request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        var spec = new SearchCustomerSpecs(request);

        var items = await repository.ListAsync(spec, cancellationToken).ConfigureAwait(false);
        var totalCount = await repository.CountAsync(spec, cancellationToken).ConfigureAwait(false);

        return new PagedList<CustomerResponse>(items, request!.PageNumber, request!.PageSize, totalCount);
    }
}

