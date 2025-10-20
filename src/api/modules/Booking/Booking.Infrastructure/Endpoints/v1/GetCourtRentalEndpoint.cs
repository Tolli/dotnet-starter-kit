using FSH.Framework.Infrastructure.Auth.Policy;
using FSH.Starter.WebApi.Booking.Application.CourtRentals.Get.v1;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace FSH.Starter.WebApi.Booking.Infrastructure.Endpoints.v1;
public static class GetCourtRentalEndpoint
{
    internal static RouteHandlerBuilder MapGetCourtRentalEndpoint(this IEndpointRouteBuilder endpoints)
    {
        return endpoints
            .MapGet("/{id:guid}", async (Guid id, ISender mediator) =>
            {
                var response = await mediator.Send(new GetCourtRentalRequest(id));
                return Results.Ok(response);
            })
            .WithName(nameof(GetCourtRentalEndpoint))
            .WithSummary("gets courtrental by id")
            .WithDescription("gets prodct by id")
            .Produces<CourtRentalResponse>()
            .RequirePermission("Permissions.CourtRentals.View")
            .MapToApiVersion(1);
    }
}
