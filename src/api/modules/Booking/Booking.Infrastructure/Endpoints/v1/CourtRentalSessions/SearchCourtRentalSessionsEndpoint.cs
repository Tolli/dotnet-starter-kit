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

public static class SearchCourtRentalSessionsEndpoint
{
    internal static RouteHandlerBuilder MapGetCourtRentalSessionListEndpoint(this IEndpointRouteBuilder endpoints)
    {
        return endpoints
            .MapPost("/search", async (ISender mediator, [FromBody] SearchCourtRentalSessionsCommand command) =>
            {
                var response = await mediator.Send(command);
                return Results.Ok(response);
            })
            .WithName(nameof(SearchCourtRentalSessionsEndpoint))
            .WithSummary("Gets a list of courtrental sessions")
            .WithDescription("Gets a list of courtrental sessions with pagination and filtering support")
            .Produces<PagedList<CourtRentalSessionResponse>>()
            .RequirePermission("Permissions.CourtRentals.View")
            .MapToApiVersion(1);
    }
}

public static class SearchCourtRentalSessionsByDateCourtEndpoint
{
    internal static RouteHandlerBuilder MapGetCourtRentalSessionListByDateCourtEndpoint(this IEndpointRouteBuilder endpoints)
    {
        return endpoints
            .MapPost("/searchbydatecourt", async (ISender mediator, [FromBody] SearchCourtRentalSessionsByDateCourtCommand command) =>
            {
                var response = await mediator.Send(command);
                return Results.Ok(response);
            })
            .WithName(nameof(SearchCourtRentalSessionsByDateCourtEndpoint))
            .WithSummary("Gets a list of courtrental sessions by house weekday time court")
            .WithDescription("Gets a list of courtrental sessions with pagination and filtering support")
            .Produces<PagedList<CourtRentalSessionResponse>>()
            .RequirePermission("Permissions.CourtRentals.View")
            .MapToApiVersion(1);
    }
}

