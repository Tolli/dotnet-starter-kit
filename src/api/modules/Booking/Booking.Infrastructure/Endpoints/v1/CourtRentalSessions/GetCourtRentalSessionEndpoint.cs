using FSH.Framework.Infrastructure.Auth.Policy;
using FSH.Starter.WebApi.Booking.Application.CourtRentals.Get.v1;
using FSH.Starter.WebApi.Booking.Application.CourtRentalSessions.Get.v1;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace FSH.Starter.WebApi.Booking.Infrastructure.Endpoints.v1;
public static class GetCourtRentalSessionEndpoint
{
    internal static RouteHandlerBuilder MapGetCourtRentalSessionEndpoint(this IEndpointRouteBuilder endpoints)
    {
        return endpoints
            .MapGet("/{id:guid}", async (Guid id, ISender mediator) =>
            {
                var response = await mediator.Send(new GetCourtRentalSessionRequest(id));
                return Results.Ok(response);
            })
            .WithName(nameof(GetCourtRentalSessionEndpoint))
            .WithSummary("gets courtrental session by id")
            .WithDescription("gets courtrental session by id")
            .Produces<CourtRentalSessionResponse>()
            .RequirePermission("Permissions.CourtRentals.View")
            .MapToApiVersion(1);
    }
}
