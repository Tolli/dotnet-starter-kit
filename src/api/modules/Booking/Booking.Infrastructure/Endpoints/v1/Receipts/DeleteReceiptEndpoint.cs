using FSH.Framework.Infrastructure.Auth.Policy;
using FSH.Starter.WebApi.Booking.Application.Receipts.Delete.v1;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace FSH.Starter.WebApi.Booking.Infrastructure.Endpoints.v1;
public static class DeleteReceiptEndpoint
{
    internal static RouteHandlerBuilder MapReceiptDeleteEndpoint(this IEndpointRouteBuilder endpoints)
    {
        return endpoints
            .MapDelete("/{id:guid}", async (Guid id, ISender mediator) =>
             {
                 await mediator.Send(new DeleteReceiptCommand(id));
                 return Results.NoContent();
             })
            .WithName(nameof(DeleteReceiptEndpoint))
            .WithSummary("deletes Receipt by id")
            .WithDescription("deletes Receipt by id")
            .Produces(StatusCodes.Status204NoContent)
            .RequirePermission("Permissions.Receipts.Delete")
            .MapToApiVersion(1);
    }
}
