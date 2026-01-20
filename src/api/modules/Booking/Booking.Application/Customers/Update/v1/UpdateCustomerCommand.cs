using MediatR;

namespace FSH.Starter.WebApi.Booking.Application.Customers.Update.v1;
public sealed record UpdateCustomerCommand(
    Guid Id,
    string? Name,
    string? ClubNumber,
    string? Ssn,
    string? Address,
    string? Notes,
    string? Email,
    string? PhoneNumber,
    string? PostalCode
    ) : IRequest<UpdateCustomerResponse>;
