using FSH.Framework.Core.Paging;
using FSH.Framework.Infrastructure.Auth.Policy;
using FSH.Starter.WebApi.Booking.Application.CourtRentalShares.Get.v1;
using FSH.Starter.WebApi.Booking.Application.CourtRentalShares.Search.v1;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

namespace FSH.Starter.WebApi.Booking.Infrastructure.Endpoints.v1;

public static class SearchCourtRentalSharesEndpoint
{
    internal static RouteHandlerBuilder MapGetCourtRentalShareListEndpoint(this IEndpointRouteBuilder endpoints)
    {
        return endpoints
            .MapPost("/search", async (ISender mediator, [FromBody] SearchCourtRentalSharesCommand command) =>
            {
                var response = await mediator.Send(command);
                return Results.Ok(response);
            })
            .WithName(nameof(SearchCourtRentalSharesEndpoint))
            .WithSummary("Gets a list of CourtRentalShares")
            .WithDescription("Gets a list of CourtRentalShares with pagination and filtering support")
            .Produces<PagedList<CourtRentalShareResponse>>()
            .RequirePermission("Permissions.CourtRentalShares.View")
            .MapToApiVersion(1);
    }
}

