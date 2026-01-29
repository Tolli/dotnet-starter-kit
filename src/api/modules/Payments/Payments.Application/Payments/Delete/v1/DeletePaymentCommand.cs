using MediatR;

namespace FSH.Starter.WebApi.Payments.Application.Payments.Delete.v1;

public sealed record DeletePaymentCommand(Guid Id) : IRequest;
