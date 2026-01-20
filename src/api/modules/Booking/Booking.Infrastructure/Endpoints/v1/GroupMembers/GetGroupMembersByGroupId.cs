using FSH.Framework.Core.Paging;
using FSH.Framework.Infrastructure.Auth.Policy;
using FSH.Starter.WebApi.Booking.Application.GroupMembers.Get.v1;
using FSH.Starter.WebApi.Booking.Application.GroupMembers.Search.v1;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

namespace FSH.Starter.WebApi.Booking.Infrastructure.Endpoints.v1;

public static class GetGroupMembersByGroupIdEndpoint
{
    internal static RouteHandlerBuilder MapGetGroupMembersByGroupIdEndpoint(this IEndpointRouteBuilder endpoints)
    {
        return endpoints
            .MapPost("/{groupId}/members", async (ISender mediator,Guid groupId, [FromBody] GetGroupMembersByGroupIdCommand command) =>
            {
                command.GroupId = groupId;
                var response = await mediator.Send(command);
                return Results.Ok(response);
            })
            .WithName(nameof(GetGroupMembersByGroupIdEndpoint))
            .WithSummary("Gets a list of groupmembers by group id")
            .WithDescription("Gets a list of groupmembers with pagination and filtering support")
            .Produces<PagedList<GroupMemberResponse>>()
            .RequirePermission("Permissions.GroupMembers.View")
            .MapToApiVersion(1);
    }
}

