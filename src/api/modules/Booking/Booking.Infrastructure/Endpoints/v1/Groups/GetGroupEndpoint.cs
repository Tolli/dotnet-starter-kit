using FSH.Framework.Infrastructure.Auth.Policy;
using FSH.Starter.WebApi.Booking.Application.Groups.Get.v1;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace FSH.Starter.WebApi.Booking.Infrastructure.Endpoints.v1;
public static class GetGroupEndpoint
{
    internal static RouteHandlerBuilder MapGetGroupEndpoint(this IEndpointRouteBuilder endpoints)
    {
        return endpoints
            .MapGet("/{id:guid}", async (Guid id, ISender mediator) =>
            {
                var response = await mediator.Send(new GetGroupRequest(id));
                return Results.Ok(response);
            })
            .WithName(nameof(GetGroupEndpoint))
            .WithSummary("gets group by id")
            .WithDescription("gets group by id")
            .Produces<GroupResponse>()
            .RequirePermission("Permissions.Groups.View")
            .MapToApiVersion(1);
    }
}
