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

public static class SearchGroupMembersEndpoint
{
    internal static RouteHandlerBuilder MapGetGroupMemberListEndpoint(this IEndpointRouteBuilder endpoints)
    {
        return endpoints
            .MapPost("/search", async (ISender mediator, [FromBody] SearchGroupMembersCommand command) =>
            {
                var response = await mediator.Send(command);
                return Results.Ok(response);
            })
            .WithName(nameof(SearchGroupMembersEndpoint))
            .WithSummary("Gets a list of groupmembers")
            .WithDescription("Gets a list of groupmembers with pagination and filtering support")
            .Produces<PagedList<GroupMemberResponse>>()
            .RequirePermission("Permissions.GroupMembers.View")
            .MapToApiVersion(1);
    }
}

