using FSH.Framework.Infrastructure.Auth.Policy;
using FSH.Starter.WebApi.Booking.Application.CourtRentalSessions.Update.v1;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace FSH.Starter.WebApi.Booking.Infrastructure.Endpoints.v1;
public static class UpdateCourtRentalSessionEndpoint
{
    internal static RouteHandlerBuilder MapCourtRentalSessionUpdateEndpoint(this IEndpointRouteBuilder endpoints)
    {
        return endpoints
            .MapPut("/{id:guid}", async (Guid id, UpdateCourtRentalSessionCommand request, ISender mediator) =>
            {
                if (id != request.Id) return Results.BadRequest();
                var response = await mediator.Send(request);
                return Results.Ok(response);
            })
            .WithName(nameof(UpdateCourtRentalSessionEndpoint))
            .WithSummary("update a courtrental session")
            .WithDescription("update a courtrental session")
            .Produces<UpdateCourtRentalSessionResponse>()
            .RequirePermission("Permissions.CourtRentals.Update")
            .MapToApiVersion(1);
    }
}
