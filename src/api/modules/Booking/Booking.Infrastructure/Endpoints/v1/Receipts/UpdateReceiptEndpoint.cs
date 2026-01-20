using FSH.Framework.Infrastructure.Auth.Policy;
using FSH.Starter.WebApi.Booking.Application.Receipts.Update.v1;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace FSH.Starter.WebApi.Booking.Infrastructure.Endpoints.v1;
public static class UpdateReceiptEndpoint
{
    internal static RouteHandlerBuilder MapReceiptUpdateEndpoint(this IEndpointRouteBuilder endpoints)
    {
        return endpoints
            .MapPut("/{id:guid}", async (Guid id, UpdateReceiptCommand request, ISender mediator) =>
            {
                if (id != request.Id) return Results.BadRequest();
                var response = await mediator.Send(request);
                return Results.Ok(response);
            })
            .WithName(nameof(UpdateReceiptEndpoint))
            .WithSummary("update a Receipt")
            .WithDescription("update a Receipt")
            .Produces<UpdateReceiptResponse>()
            .RequirePermission("Permissions.Receipts.Update")
            .MapToApiVersion(1);
    }
}
