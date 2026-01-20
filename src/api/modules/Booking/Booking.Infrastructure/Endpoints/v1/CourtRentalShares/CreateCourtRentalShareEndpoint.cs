using FSH.Framework.Infrastructure.Auth.Policy;
using FSH.Starter.WebApi.Booking.Application.CourtRentalShares.Create.v1;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace FSH.Starter.WebApi.Booking.Infrastructure.Endpoints.v1;
public static class CreateCourtRentalShareEndpoint
{
    internal static RouteHandlerBuilder MapCourtRentalShareCreationEndpoint(this IEndpointRouteBuilder endpoints)
    {
        return endpoints
            .MapPost("/", async (CreateCourtRentalShareCommand request, ISender mediator) =>
            {
                var response = await mediator.Send(request);
                return Results.Ok(response);
            })
            .WithName(nameof(CreateCourtRentalShareEndpoint))
            .WithSummary("creates a CourtRentalShare")
            .WithDescription("creates a CourtRentalShare")
            .Produces<CreateCourtRentalShareResponse>()
            .RequirePermission("Permissions.CourtRentalShares.Create")
            .MapToApiVersion(1);
    }
}
