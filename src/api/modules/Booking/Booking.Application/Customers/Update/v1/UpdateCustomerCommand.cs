using MediatR;

namespace FSH.Starter.WebApi.Booking.Application.Customers.Update.v1;
public sealed record UpdateCustomerCommand(
    Guid Id,
    string? Name,
    decimal Price,
    string? Description = null,
    Guid? GroupId = null) : IRequest<UpdateCustomerResponse>;
