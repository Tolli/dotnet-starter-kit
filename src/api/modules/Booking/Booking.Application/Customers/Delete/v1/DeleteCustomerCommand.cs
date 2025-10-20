using MediatR;

namespace FSH.Starter.WebApi.Booking.Application.Customers.Delete.v1;
public sealed record DeleteCustomerCommand(
    Guid Id) : IRequest;
