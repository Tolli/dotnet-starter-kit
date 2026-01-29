using FSH.Framework.Infrastructure.Auth.Policy;
using FSH.Starter.WebApi.Payments.Application.Payments.Delete.v1;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace FSH.Starter.WebApi.Payments.Infrastructure.Endpoints.v1;

public static class DeletePaymentEndpoint
{
    internal static RouteHandlerBuilder MapPaymentDeleteEndpoint(this IEndpointRouteBuilder endpoints)
    {
        return endpoints
            .MapDelete("/{id:guid}", async (Guid id, ISender mediator) =>
            {
                await mediator.Send(new DeletePaymentCommand(id));
                return Results.NoContent();
            })
            .WithName(nameof(DeletePaymentEndpoint))
            .WithSummary("deletes a payment")
            .WithDescription("deletes a payment")
            .RequirePermission("Permissions.Payments.Delete")
            .MapToApiVersion(1);
    }
}
