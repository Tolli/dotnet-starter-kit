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

public static class GetCourtRentalsByGroupIdEndpoint
{
    internal static RouteHandlerBuilder MapGetCourtRentalsByGroupIdEndpoint(this IEndpointRouteBuilder endpoints)
    {
        return endpoints
            .MapPost("/{groupId}/courtrentals", async (ISender mediator,Guid groupId, [FromBody] GetCourtRentalsByGroupIdCommand command) =>
            {
                command.GroupId = groupId;
                var response = await mediator.Send(command);
                return Results.Ok(response);
            })
            .WithName(nameof(GetCourtRentalsByGroupIdEndpoint))
            .WithSummary("Gets a list of CourtRentals by group id")
            .WithDescription("Gets a list of CourtRentals with pagination and filtering support")
            .Produces<PagedList<CourtRentalResponse>>()
            .RequirePermission("Permissions.CourtRentals.View")
            .MapToApiVersion(1);
    }
}

