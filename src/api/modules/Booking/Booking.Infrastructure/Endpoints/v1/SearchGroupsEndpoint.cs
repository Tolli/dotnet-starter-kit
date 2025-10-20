using FSH.Framework.Core.Paging;
using FSH.Framework.Infrastructure.Auth.Policy;
using FSH.Starter.WebApi.Booking.Application.Groups.Get.v1;
using FSH.Starter.WebApi.Booking.Application.Groups.Search.v1;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

namespace FSH.Starter.WebApi.Booking.Infrastructure.Endpoints.v1;

public static class SearchGroupsEndpoint
{
    internal static RouteHandlerBuilder MapGetGroupListEndpoint(this IEndpointRouteBuilder endpoints)
    {
        return endpoints
            .MapPost("/search", async (ISender mediator, [FromBody] SearchGroupsCommand command) =>
            {
                var response = await mediator.Send(command);
                return Results.Ok(response);
            })
            .WithName(nameof(SearchGroupsEndpoint))
            .WithSummary("Gets a list of groups")
            .WithDescription("Gets a list of groups with pagination and filtering support")
            .Produces<PagedList<GroupResponse>>()
            .RequirePermission("Permissions.Groups.View")
            .MapToApiVersion(1);
    }
}
