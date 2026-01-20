using FSH.Framework.Infrastructure.Auth.Policy;
using FSH.Starter.WebApi.Booking.Application.CourtRentalShares.Delete.v1;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace FSH.Starter.WebApi.Booking.Infrastructure.Endpoints.v1;
public static class DeleteCourtRentalShareEndpoint
{
    internal static RouteHandlerBuilder MapCourtRentalShareDeleteEndpoint(this IEndpointRouteBuilder endpoints)
    {
        return endpoints
            .MapDelete("/{id:guid}", async (Guid id, ISender mediator) =>
             {
                 await mediator.Send(new DeleteCourtRentalShareCommand(id));
                 return Results.NoContent();
             })
            .WithName(nameof(DeleteCourtRentalShareEndpoint))
            .WithSummary("deletes CourtRentalShare by id")
            .WithDescription("deletes CourtRentalShare by id")
            .Produces(StatusCodes.Status204NoContent)
            .RequirePermission("Permissions.CourtRentalShares.Delete")
            .MapToApiVersion(1);
    }
}
