using FSH.Framework.Infrastructure.Auth.Policy;
using FSH.Starter.WebApi.Booking.Application.CourtRentals.Delete.v1;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace FSH.Starter.WebApi.Booking.Infrastructure.Endpoints.v1;
public static class DeleteCourtRentalEndpoint
{
    internal static RouteHandlerBuilder MapCourtRentalDeleteEndpoint(this IEndpointRouteBuilder endpoints)
    {
        return endpoints
            .MapDelete("/{id:guid}", async (Guid id, ISender mediator) =>
             {
                 await mediator.Send(new DeleteCourtRentalCommand(id));
                 return Results.NoContent();
             })
            .WithName(nameof(DeleteCourtRentalEndpoint))
            .WithSummary("deletes courtrental by id")
            .WithDescription("deletes courtrental by id")
            .Produces(StatusCodes.Status204NoContent)
            .RequirePermission("Permissions.CourtRentals.Delete")
            .MapToApiVersion(1);
    }
}
