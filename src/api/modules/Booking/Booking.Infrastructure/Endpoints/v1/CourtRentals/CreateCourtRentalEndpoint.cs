using FSH.Framework.Infrastructure.Auth.Policy;
using FSH.Starter.WebApi.Booking.Application.CourtRentals.Create.v1;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace FSH.Starter.WebApi.Booking.Infrastructure.Endpoints.v1;
public static class CreateCourtRentalEndpoint
{
    internal static RouteHandlerBuilder MapCourtRentalCreationEndpoint(this IEndpointRouteBuilder endpoints)
    {
        return endpoints
            .MapPost("/", async (CreateCourtRentalCommand request, ISender mediator) =>
            {
                var response = await mediator.Send(request);
                return Results.Ok(response);
            })
            .WithName(nameof(CreateCourtRentalEndpoint))
            .WithSummary("creates a courtrental")
            .WithDescription("creates a courtrental")
            .Produces<CreateCourtRentalResponse>()
            .RequirePermission("Permissions.CourtRentals.Create")
            .MapToApiVersion(1);
    }
}
