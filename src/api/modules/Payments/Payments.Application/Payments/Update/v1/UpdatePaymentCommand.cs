using MediatR;

namespace FSH.Starter.WebApi.Payments.Application.Payments.Update.v1;

public sealed record UpdatePaymentCommand(
    Guid Id,
    decimal? Amount = null,
    string? Description = null,
    string? Currency = null) : IRequest<UpdatePaymentResponse>;
