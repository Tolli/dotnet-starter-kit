using MediatR;

namespace FSH.Starter.WebApi.Payments.Application.Payments.Get.v1;

public sealed record GetPaymentRequest(Guid Id) : IRequest<PaymentResponse>;
