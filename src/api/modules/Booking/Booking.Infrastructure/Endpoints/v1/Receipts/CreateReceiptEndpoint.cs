using FSH.Framework.Infrastructure.Auth.Policy;
using FSH.Starter.WebApi.Booking.Application.Receipts.Create.v1;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace FSH.Starter.WebApi.Booking.Infrastructure.Endpoints.v1;
public static class CreateReceiptEndpoint
{
    internal static RouteHandlerBuilder MapReceiptCreationEndpoint(this IEndpointRouteBuilder endpoints)
    {
        return endpoints
            .MapPost("/", async (CreateReceiptCommand request, ISender mediator) =>
            {
                var response = await mediator.Send(request);
                return Results.Ok(response);
            })
            .WithName(nameof(CreateReceiptEndpoint))
            .WithSummary("creates a Receipt")
            .WithDescription("creates a Receipt")
            .Produces<CreateReceiptResponse>()
            .RequirePermission("Permissions.Receipts.Create")
            .MapToApiVersion(1);
    }
}
