using FSH.Framework.Infrastructure.Auth.Policy;
using FSH.Starter.WebApi.Booking.Application.CourtRentals.Create.v1;
using FSH.Starter.WebApi.Booking.Application.CourtRentalSessions.Create.v1;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace FSH.Starter.WebApi.Booking.Infrastructure.Endpoints.v1;
public static class CreateCourtRentalSessionEndpoint
{
    internal static RouteHandlerBuilder MapCourtRentalSessionCreationEndpoint(this IEndpointRouteBuilder endpoints)
    {
        return endpoints
            .MapPost("/", async (CreateCourtRentalSessionCommand request, ISender mediator) =>
            {
                var response = await mediator.Send(request);
                return Results.Ok(response);
            })
            .WithName(nameof(CreateCourtRentalSessionEndpoint))
            .WithSummary("creates a courtrental session")
            .WithDescription("creates a courtrental session")
            .Produces<CreateCourtRentalSessionResponse>()
            .RequirePermission("Permissions.CourtRentals.Create")
            .MapToApiVersion(1);
    }
}
