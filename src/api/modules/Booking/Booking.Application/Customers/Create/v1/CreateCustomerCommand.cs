using System.ComponentModel;
using MediatR;

namespace FSH.Starter.WebApi.Booking.Application.Customers.Create.v1;
public sealed record CreateCustomerCommand(
    [property: DefaultValue("Sample Customer")] string? Name,
    [property: DefaultValue(10)] decimal Price,
    [property: DefaultValue("Descriptive Description")] string? Description = null,
    [property: DefaultValue(null)] Guid? GroupId = null) : IRequest<CreateCustomerResponse>;
