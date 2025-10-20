using FSH.Framework.Infrastructure.Auth.Policy;
using FSH.Starter.WebApi.Booking.Application.Groups.Delete.v1;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace FSH.Starter.WebApi.Booking.Infrastructure.Endpoints.v1;
public static class DeleteGroupEndpoint
{
    internal static RouteHandlerBuilder MapGroupDeleteEndpoint(this IEndpointRouteBuilder endpoints)
    {
        return endpoints
            .MapDelete("/{id:guid}", async (Guid id, ISender mediator) =>
             {
                 await mediator.Send(new DeleteGroupCommand(id));
                 return Results.NoContent();
             })
            .WithName(nameof(DeleteGroupEndpoint))
            .WithSummary("deletes group by id")
            .WithDescription("deletes group by id")
            .Produces(StatusCodes.Status204NoContent)
            .RequirePermission("Permissions.Groups.Delete")
            .MapToApiVersion(1);
    }
}
