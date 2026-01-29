using FSH.Framework.Core.Paging;
using FSH.Framework.Infrastructure.Auth.Policy;
using FSH.Starter.WebApi.Payments.Application.Payments.Get.v1;
using FSH.Starter.WebApi.Payments.Application.Payments.Search.v1;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace FSH.Starter.WebApi.Payments.Infrastructure.Endpoints.v1;

public static class SearchPaymentsEndpoint
{
    internal static RouteHandlerBuilder MapGetPaymentListEndpoint(this IEndpointRouteBuilder endpoints)
    {
        return endpoints
            .MapPost("/search", async (SearchPaymentsCommand request, ISender mediator) =>
            {
                var response = await mediator.Send(request);
                return Results.Ok(response);
            })
            .WithName(nameof(SearchPaymentsEndpoint))
            .WithSummary("searches payments")
            .WithDescription("searches payments")
            .Produces<PagedList<PaymentResponse>>()
            .RequirePermission("Permissions.Payments.View")
            .MapToApiVersion(1);
    }
}
