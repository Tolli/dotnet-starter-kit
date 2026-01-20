using FSH.Framework.Infrastructure.Auth.Policy;
using FSH.Starter.WebApi.Booking.Application.GroupMembers.Delete.v1;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace FSH.Starter.WebApi.Booking.Infrastructure.Endpoints.v1;
public static class DeleteGroupMemberEndpoint
{
    internal static RouteHandlerBuilder MapGroupMemberDeleteEndpoint(this IEndpointRouteBuilder endpoints)
    {
        return endpoints
            .MapDelete("/{id:guid}", async (Guid id, ISender mediator) =>
             {
                 await mediator.Send(new DeleteGroupMemberCommand(id));
                 return Results.NoContent();
             })
            .WithName(nameof(DeleteGroupMemberEndpoint))
            .WithSummary("deletes groupmember by id")
            .WithDescription("deletes groupmember by id")
            .Produces(StatusCodes.Status204NoContent)
            .RequirePermission("Permissions.GroupMembers.Delete")
            .MapToApiVersion(1);
    }
}
