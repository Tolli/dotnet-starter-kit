using FSH.Framework.Core.Persistence;
using FSH.Starter.WebApi.Booking.Domain;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace FSH.Starter.WebApi.Booking.Application.Customers.Create.v1;
public sealed class CreateCustomerHandler(
    ILogger<CreateCustomerHandler> logger,
    [FromKeyedServices("booking:customers")] IRepository<Customer> repository)
    : IRequestHandler<CreateCustomerCommand, CreateCustomerResponse>
{
    public async Task<CreateCustomerResponse> Handle(CreateCustomerCommand request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);
        var customer = Customer.Create(request.Name!, request.ClubNumber, request.Ssn, request.Address, request.Notes, request.Email, request.PhoneNumber, request.PostalCode);
        await repository.AddAsync(customer, cancellationToken);
        logger.LogInformation("customer created {CustomerId}", customer.Id);
        return new CreateCustomerResponse(customer.Id);
    }
}
