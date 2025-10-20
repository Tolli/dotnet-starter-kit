using Microsoft.Extensions.DependencyInjection;
using FSH.Starter.WebApi.Booking.Domain.Exceptions;
using FSH.Framework.Core.Persistence;
using FSH.Framework.Core.Caching;
using FSH.Starter.WebApi.Booking.Domain;
using MediatR;

namespace FSH.Starter.WebApi.Booking.Application.Customers.Get.v1;
public sealed class GetCustomerHandler(
    [FromKeyedServices("booking:customers")] IReadRepository<Customer> repository,
    ICacheService cache)
    : IRequestHandler<GetCustomerRequest, CustomerResponse>
{
    public async Task<CustomerResponse> Handle(GetCustomerRequest request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);
        var item = await cache.GetOrSetAsync(
            $"customer:{request.Id}",
            async () =>
            {
                var spec = new GetCustomerSpecs(request.Id);
                var customerItem = await repository.FirstOrDefaultAsync(spec, cancellationToken);
                if (customerItem == null) throw new CustomerNotFoundException(request.Id);
                return customerItem;
            },
            cancellationToken: cancellationToken);
        return item!;
    }
}
