using FSH.Framework.Infrastructure.Auth.Policy;
using FSH.Starter.WebApi.Booking.Application.Groups.Update.v1;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace FSH.Starter.WebApi.Booking.Infrastructure.Endpoints.v1;
public static class UpdateGroupEndpoint
{
    internal static RouteHandlerBuilder MapGroupUpdateEndpoint(this IEndpointRouteBuilder endpoints)
    {
        return endpoints
            .MapPut("/{id:guid}", async (Guid id, UpdateGroupCommand request, ISender mediator) =>
            {
                if (id != request.Id) return Results.BadRequest();
                var response = await mediator.Send(request);
                return Results.Ok(response);
            })
            .WithName(nameof(UpdateGroupEndpoint))
            .WithSummary("update a group")
            .WithDescription("update a group")
            .Produces<UpdateGroupResponse>()
            .RequirePermission("Permissions.Groups.Update")
            .MapToApiVersion(1);
    }
}
