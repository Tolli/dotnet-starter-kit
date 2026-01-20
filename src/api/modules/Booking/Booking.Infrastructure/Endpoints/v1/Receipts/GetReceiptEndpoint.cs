using FSH.Framework.Infrastructure.Auth.Policy;
using FSH.Starter.WebApi.Booking.Application.Receipts.Get.v1;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace FSH.Starter.WebApi.Booking.Infrastructure.Endpoints.v1;
public static class GetReceiptEndpoint
{
    internal static RouteHandlerBuilder MapGetReceiptEndpoint(this IEndpointRouteBuilder endpoints)
    {
        return endpoints
            .MapGet("/{id:guid}", async (Guid id, ISender mediator) =>
            {
                var response = await mediator.Send(new GetReceiptRequest(id));
                return Results.Ok(response);
            })
            .WithName(nameof(GetReceiptEndpoint))
            .WithSummary("gets Receipt by id")
            .WithDescription("gets Receipt by id")
            .Produces<ReceiptResponse>()
            .RequirePermission("Permissions.Receipts.View")
            .MapToApiVersion(1);
    }
}
