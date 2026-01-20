using FSH.Framework.Infrastructure.Auth.Policy;
using FSH.Starter.WebApi.Booking.Application.Groups.Create.v1;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace FSH.Starter.WebApi.Booking.Infrastructure.Endpoints.v1;
public static class CreateGroupEndpoint
{
    internal static RouteHandlerBuilder MapGroupCreationEndpoint(this IEndpointRouteBuilder endpoints)
    {
        return endpoints
            .MapPost("/", async (CreateGroupCommand request, ISender mediator) =>
            {
                var response = await mediator.Send(request);
                return Results.Ok(response);
            })
            .WithName(nameof(CreateGroupEndpoint))
            .WithSummary("creates a group")
            .WithDescription("creates a group")
            .Produces<CreateGroupResponse>()
            .RequirePermission("Permissions.Groups.Create")
            .MapToApiVersion(1);
    }
}
