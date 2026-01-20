using FSH.Framework.Infrastructure.Auth.Policy;
using FSH.Starter.WebApi.Booking.Application.CourtRentalShares.Update.v1;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace FSH.Starter.WebApi.Booking.Infrastructure.Endpoints.v1;
public static class UpdateCourtRentalShareEndpoint
{
    internal static RouteHandlerBuilder MapCourtRentalShareUpdateEndpoint(this IEndpointRouteBuilder endpoints)
    {
        return endpoints
            .MapPut("/{id:guid}", async (Guid id, UpdateCourtRentalShareCommand request, ISender mediator) =>
            {
                if (id != request.Id) return Results.BadRequest();
                var response = await mediator.Send(request);
                return Results.Ok(response);
            })
            .WithName(nameof(UpdateCourtRentalShareEndpoint))
            .WithSummary("update a CourtRentalShare")
            .WithDescription("update a CourtRentalShare")
            .Produces<UpdateCourtRentalShareResponse>()
            .RequirePermission("Permissions.CourtRentalShares.Update")
            .MapToApiVersion(1);
    }
}
