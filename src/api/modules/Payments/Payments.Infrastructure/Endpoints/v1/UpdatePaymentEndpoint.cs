using FSH.Framework.Infrastructure.Auth.Policy;
using FSH.Starter.WebApi.Payments.Application.Payments.Update.v1;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace FSH.Starter.WebApi.Payments.Infrastructure.Endpoints.v1;

public static class UpdatePaymentEndpoint
{
    internal static RouteHandlerBuilder MapPaymentUpdateEndpoint(this IEndpointRouteBuilder endpoints)
    {
        return endpoints
            .MapPut("/{id:guid}", async (Guid id, UpdatePaymentCommand request, ISender mediator) =>
            {
                if (id != request.Id)
                    return Results.BadRequest("Route ID must match request ID");

                var response = await mediator.Send(request);
                return Results.Ok(response);
            })
            .WithName(nameof(UpdatePaymentEndpoint))
            .WithSummary("updates a payment")
            .WithDescription("updates a payment")
            .Produces<UpdatePaymentResponse>()
            .RequirePermission("Permissions.Payments.Update")
            .MapToApiVersion(1);
    }
}
