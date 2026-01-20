using FSH.Framework.Infrastructure.Auth.Policy;
using FSH.Starter.WebApi.Booking.Application.CourtRentalShares.Get.v1;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace FSH.Starter.WebApi.Booking.Infrastructure.Endpoints.v1;
public static class GetCourtRentalShareEndpoint
{
    internal static RouteHandlerBuilder MapGetCourtRentalShareEndpoint(this IEndpointRouteBuilder endpoints)
    {
        return endpoints
            .MapGet("/{id:guid}", async (Guid id, ISender mediator) =>
            {
                var response = await mediator.Send(new GetCourtRentalShareRequest(id));
                return Results.Ok(response);
            })
            .WithName(nameof(GetCourtRentalShareEndpoint))
            .WithSummary("gets CourtRentalShare by id")
            .WithDescription("gets CourtRentalShare by id")
            .Produces<CourtRentalShareResponse>()
            .RequirePermission("Permissions.CourtRentalShares.View")
            .MapToApiVersion(1);
    }
}
