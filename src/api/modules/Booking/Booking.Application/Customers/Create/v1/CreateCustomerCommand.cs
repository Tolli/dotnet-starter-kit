using System.ComponentModel;
using MediatR;

namespace FSH.Starter.WebApi.Booking.Application.Customers.Create.v1;
public sealed record CreateCustomerCommand(
    [property: DefaultValue("Sample Customer")] string Name,
    [property: DefaultValue("")] string ClubNumber,
    [property: DefaultValue("")] string? Ssn = null,
    [property: DefaultValue("")] string? Address = null,
    [property: DefaultValue("")] string? Notes = null,
    [property: DefaultValue("")] string? Email = null,
    [property: DefaultValue("")] string? PhoneNumber = null,
    [property: DefaultValue("")] string? PostalCode = null) : IRequest<CreateCustomerResponse>;
