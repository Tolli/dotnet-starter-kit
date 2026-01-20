using FSH.Framework.Core.Paging;
using FSH.Framework.Infrastructure.Auth.Policy;
using FSH.Starter.WebApi.Booking.Application.CourtRentals.Get.v1;
using FSH.Starter.WebApi.Booking.Application.CourtRentals.Search.v1;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

namespace FSH.Starter.WebApi.Booking.Infrastructure.Endpoints.v1;

public static class SearchCourtRentalsEndpoint
{
    internal static RouteHandlerBuilder MapGetCourtRentalListEndpoint(this IEndpointRouteBuilder endpoints)
    {
        return endpoints
            .MapPost("/search", async (ISender mediator, [FromBody] SearchCourtRentalsCommand command) =>
            {
                var response = await mediator.Send(command);
                return Results.Ok(response);
            })
            .WithName(nameof(SearchCourtRentalsEndpoint))
            .WithSummary("Gets a list of courtrentals")
            .WithDescription("Gets a list of courtrentals with pagination and filtering support")
            .Produces<PagedList<CourtRentalResponse>>()
            .RequirePermission("Permissions.CourtRentals.View")
            .MapToApiVersion(1);
    }
}

public static class SearchCourtRentalsByHouseDayTimeCourtEndpoint
{
    internal static RouteHandlerBuilder MapGetCourtRentalListByHouseDayTimeCourtEndpoint(this IEndpointRouteBuilder endpoints)
    {
        return endpoints
            .MapPost("/searchbydaytimecourt", async (ISender mediator, [FromBody] SearchCourtRentalsByHouseDayTimeCourtCommand command) =>
            {
                var response = await mediator.Send(command);
                return Results.Ok(response);
            })
            .WithName(nameof(SearchCourtRentalsByHouseDayTimeCourtEndpoint))
            .WithSummary("Gets a list of courtrentals by house weekday time court")
            .WithDescription("Gets a list of courtrentals with pagination and filtering support")
            .Produces<PagedList<CourtRentalResponse>>()
            .RequirePermission("Permissions.CourtRentals.View")
            .MapToApiVersion(1);
    }
}

