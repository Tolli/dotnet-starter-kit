using FSH.Framework.Infrastructure.Auth.Policy;
using FSH.Starter.WebApi.Payments.Application.Payments.Get.v1;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace FSH.Starter.WebApi.Payments.Infrastructure.Endpoints.v1;

public static class GetPaymentEndpoint
{
    internal static RouteHandlerBuilder MapGetPaymentEndpoint(this IEndpointRouteBuilder endpoints)
    {
        return endpoints
            .MapGet("/{id:guid}", async (Guid id, ISender mediator) =>
            {
                var response = await mediator.Send(new GetPaymentRequest(id));
                return Results.Ok(response);
            })
            .WithName(nameof(GetPaymentEndpoint))
            .WithSummary("gets payment details")
            .WithDescription("gets payment details")
            .Produces<PaymentResponse>()
            .RequirePermission("Permissions.Payments.View")
            .MapToApiVersion(1);
    }
}
