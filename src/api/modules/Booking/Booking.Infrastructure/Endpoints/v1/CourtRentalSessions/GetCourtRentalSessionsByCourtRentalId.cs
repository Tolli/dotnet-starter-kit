using FSH.Framework.Core.Paging;
using FSH.Framework.Infrastructure.Auth.Policy;
using FSH.Starter.WebApi.Booking.Application.CourtRentalSessions.Get.v1;
using FSH.Starter.WebApi.Booking.Application.CourtRentalSessions.Search.v1;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

namespace FSH.Starter.WebApi.Booking.Infrastructure.Endpoints.v1;

public static class GetCourtRentalSessionsByCourtRentalIdEndpoint
{
    internal static RouteHandlerBuilder MapGetCourtRentalSessionsByCourtRentalIdEndpoint(this IEndpointRouteBuilder endpoints)
    {
        return endpoints
            .MapPost("/{courtRentalId}/courtrentalsessions", async (ISender mediator,Guid groupId, [FromBody] GetCourtRentalSessionsByCourtRentalIdCommand command) =>
            {
                command.CourtRentalId = groupId;
                var response = await mediator.Send(command);
                return Results.Ok(response);
            })
            .WithName(nameof(GetCourtRentalSessionsByCourtRentalIdEndpoint))
            .WithSummary("Gets a list of CourtRentalSessions by court rental id")
            .WithDescription("Gets a list of CourtRentalSessions with pagination and filtering support")
            .Produces<PagedList<CourtRentalSessionResponse>>()
            .RequirePermission("Permissions.CourtRentals.View")
            .MapToApiVersion(1);
    }
}

