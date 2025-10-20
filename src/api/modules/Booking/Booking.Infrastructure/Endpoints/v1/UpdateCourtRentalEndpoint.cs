using FSH.Framework.Infrastructure.Auth.Policy;
using FSH.Starter.WebApi.Booking.Application.CourtRentals.Update.v1;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace FSH.Starter.WebApi.Booking.Infrastructure.Endpoints.v1;
public static class UpdateCourtRentalEndpoint
{
    internal static RouteHandlerBuilder MapCourtRentalUpdateEndpoint(this IEndpointRouteBuilder endpoints)
    {
        return endpoints
            .MapPut("/{id:guid}", async (Guid id, UpdateCourtRentalCommand request, ISender mediator) =>
            {
                if (id != request.Id) return Results.BadRequest();
                var response = await mediator.Send(request);
                return Results.Ok(response);
            })
            .WithName(nameof(UpdateCourtRentalEndpoint))
            .WithSummary("update a courtrental")
            .WithDescription("update a courtrental")
            .Produces<UpdateCourtRentalResponse>()
            .RequirePermission("Permissions.CourtRentals.Update")
            .MapToApiVersion(1);
    }
}
