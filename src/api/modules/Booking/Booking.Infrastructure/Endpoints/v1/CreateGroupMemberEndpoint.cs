using FSH.Framework.Infrastructure.Auth.Policy;
using FSH.Starter.WebApi.Booking.Application.GroupMembers.Create.v1;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace FSH.Starter.WebApi.Booking.Infrastructure.Endpoints.v1;
public static class CreateGroupMemberEndpoint
{
    internal static RouteHandlerBuilder MapGroupMemberCreationEndpoint(this IEndpointRouteBuilder endpoints)
    {
        return endpoints
            .MapPost("/", async (CreateGroupMemberCommand request, ISender mediator) =>
            {
                var response = await mediator.Send(request);
                return Results.Ok(response);
            })
            .WithName(nameof(CreateGroupMemberEndpoint))
            .WithSummary("creates a groupmember")
            .WithDescription("creates a groupmember")
            .Produces<CreateGroupMemberResponse>()
            .RequirePermission("Permissions.GroupMembers.Create")
            .MapToApiVersion(1);
    }
}
