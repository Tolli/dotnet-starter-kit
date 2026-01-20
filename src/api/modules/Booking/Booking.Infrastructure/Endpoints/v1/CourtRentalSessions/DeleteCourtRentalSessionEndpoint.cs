using FSH.Framework.Infrastructure.Auth.Policy;
using FSH.Starter.WebApi.Booking.Application.CourtRentals.Delete.v1;
using FSH.Starter.WebApi.Booking.Application.CourtRentalSessions.Delete.v1;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace FSH.Starter.WebApi.Booking.Infrastructure.Endpoints.v1;
public static class DeleteCourtRentalSessionEndpoint
{
    internal static RouteHandlerBuilder MapCourtRentalSessionDeleteEndpoint(this IEndpointRouteBuilder endpoints)
    {
        return endpoints
            .MapDelete("/{id:guid}", async (Guid id, ISender mediator) =>
             {
                 await mediator.Send(new DeleteCourtRentalSessionCommand(id));
                 return Results.NoContent();
             })
            .WithName(nameof(DeleteCourtRentalSessionEndpoint))
            .WithSummary("deletes courtrental session by id")
            .WithDescription("deletes courtrental session by id")
            .Produces(StatusCodes.Status204NoContent)
            .RequirePermission("Permissions.CourtRentals.Delete")
            .MapToApiVersion(1);
    }
}
