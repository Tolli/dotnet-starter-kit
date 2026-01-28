using FSH.Framework.Infrastructure.Auth.Policy;
using FSH.Starter.WebApi.Booking.Application.Customers.Get.v1;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace FSH.Starter.WebApi.Booking.Infrastructure.Endpoints.v1;
public static class GetCustomerWithSharesEndpoint
{
    internal static RouteHandlerBuilder MapGetCustomerWithSharesEndpoint(this IEndpointRouteBuilder endpoints)
    {
        return endpoints
            .MapGet("/{id:guid}", async (Guid id, ISender mediator) =>
            {
                var response = await mediator.Send(new GetCustomerWithSharesRequest(id));
                return Results.Ok(response);
            })
            .WithName(nameof(GetCustomerWithSharesEndpoint))
            .WithSummary("gets customer by id")
            .WithDescription("gets prodct by id")
            .Produces<CustomerWithSharesResponse>()
            .RequirePermission("Permissions.Customers.View")
            .MapToApiVersion(1);
    }
}
