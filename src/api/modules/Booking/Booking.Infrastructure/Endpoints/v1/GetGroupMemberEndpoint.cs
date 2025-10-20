using FSH.Framework.Infrastructure.Auth.Policy;
using FSH.Starter.WebApi.Booking.Application.GroupMembers.Get.v1;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace FSH.Starter.WebApi.Booking.Infrastructure.Endpoints.v1;
public static class GetGroupMemberEndpoint
{
    internal static RouteHandlerBuilder MapGetGroupMemberEndpoint(this IEndpointRouteBuilder endpoints)
    {
        return endpoints
            .MapGet("/{id:guid}", async (Guid id, ISender mediator) =>
            {
                var response = await mediator.Send(new GetGroupMemberRequest(id));
                return Results.Ok(response);
            })
            .WithName(nameof(GetGroupMemberEndpoint))
            .WithSummary("gets groupmember by id")
            .WithDescription("gets groupmember by id")
            .Produces<GroupMemberResponse>()
            .RequirePermission("Permissions.GroupMembers.View")
            .MapToApiVersion(1);
    }
}
