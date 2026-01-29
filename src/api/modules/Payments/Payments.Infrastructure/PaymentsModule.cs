using Carter;
using FSH.Framework.Core.Persistence;
using FSH.Framework.Infrastructure.Persistence;
using FSH.Starter.WebApi.Payments.Domain;
using FSH.Starter.WebApi.Payments.Infrastructure.Endpoints.v1;
using FSH.Starter.WebApi.Payments.Infrastructure.Persistence;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;

namespace FSH.Starter.WebApi.Payments.Infrastructure;

public static class PaymentsModule
{
    public class Endpoints : CarterModule
    {
        public Endpoints() : base("payments") { }
        
        public override void AddRoutes(IEndpointRouteBuilder app)
        {
            var paymentGroup = app.MapGroup("payments").WithTags("payments");
            paymentGroup.MapPaymentCreationEndpoint();
            paymentGroup.MapGetPaymentEndpoint();
            paymentGroup.MapGetPaymentListEndpoint();
            paymentGroup.MapPaymentUpdateEndpoint();
            paymentGroup.MapPaymentDeleteEndpoint();
        }
    }
    
    public static WebApplicationBuilder RegisterPaymentsServices(this WebApplicationBuilder builder)
    {
        ArgumentNullException.ThrowIfNull(builder);
        
        builder.Services.BindDbContext<PaymentsDbContext>();
        builder.Services.AddScoped<IDbInitializer, PaymentsDbInitializer>();
        builder.Services.AddKeyedScoped<IRepository<Payment>, PaymentsRepository<Payment>>("payments:payments");
        builder.Services.AddKeyedScoped<IReadRepository<Payment>, PaymentsRepository<Payment>>("payments:payments");
        
        return builder;
    }

    public static WebApplication UsePaymentsModule(this WebApplication app)
    {
        return app;
    }
}
