using FSH.Framework.Core.Paging;
using FSH.Framework.Infrastructure.Auth.Policy;
using FSH.Starter.WebApi.Booking.Application.Receipts.Get.v1;
using FSH.Starter.WebApi.Booking.Application.Receipts.Search.v1;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

namespace FSH.Starter.WebApi.Booking.Infrastructure.Endpoints.v1;

public static class SearchReceiptsEndpoint
{
    internal static RouteHandlerBuilder MapGetReceiptListEndpoint(this IEndpointRouteBuilder endpoints)
    {
        return endpoints
            .MapPost("/search", async (ISender mediator, [FromBody] SearchReceiptsCommand command) =>
            {
                var response = await mediator.Send(command);
                return Results.Ok(response);
            })
            .WithName(nameof(SearchReceiptsEndpoint))
            .WithSummary("Gets a list of Receipts")
            .WithDescription("Gets a list of Receipts with pagination and filtering support")
            .Produces<PagedList<ReceiptResponse>>()
            .RequirePermission("Permissions.Receipts.View")
            .MapToApiVersion(1);
    }
}

