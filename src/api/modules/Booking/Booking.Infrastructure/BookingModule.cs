using Carter;
using FSH.Framework.Core.Persistence;
using FSH.Framework.Infrastructure.Persistence;
using FSH.Starter.WebApi.Booking.Domain;
using FSH.Starter.WebApi.Booking.Infrastructure.Endpoints.v1;
using FSH.Starter.WebApi.Booking.Infrastructure.Persistence;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;

namespace FSH.Starter.WebApi.Booking.Infrastructure;
public static class BookingModule
{
    public class Endpoints : CarterModule
    {
        public Endpoints() : base("booking") { }
        public override void AddRoutes(IEndpointRouteBuilder app)
        {
            var customerGroup = app.MapGroup("customers").WithTags("customers");
            customerGroup.MapCustomerCreationEndpoint();
            customerGroup.MapGetCustomerEndpoint();
            customerGroup.MapGetCustomerListEndpoint();
            customerGroup.MapCustomerUpdateEndpoint();
            customerGroup.MapCustomerDeleteEndpoint();

            var groupGroup = app.MapGroup("groups").WithTags("groups");
            groupGroup.MapGroupCreationEndpoint();
            groupGroup.MapGetGroupEndpoint();
            groupGroup.MapGetGroupListEndpoint();
            groupGroup.MapGroupUpdateEndpoint();
            groupGroup.MapGroupDeleteEndpoint();

            var groupMemberGroup = app.MapGroup("groupmembers").WithTags("groupmembers");
            groupMemberGroup.MapGroupMemberCreationEndpoint();
            groupMemberGroup.MapGetGroupMemberEndpoint();
            groupMemberGroup.MapGetGroupMemberListEndpoint();
            groupMemberGroup.MapGroupMemberUpdateEndpoint();
            groupMemberGroup.MapGroupMemberDeleteEndpoint();

            var courtRentalGroup = app.MapGroup("courtrentals").WithTags("courtrentals");
            courtRentalGroup.MapCourtRentalCreationEndpoint();
            courtRentalGroup.MapGetCourtRentalEndpoint();
            courtRentalGroup.MapGetCourtRentalListEndpoint();
            courtRentalGroup.MapCourtRentalUpdateEndpoint();
            courtRentalGroup.MapCourtRentalDeleteEndpoint();

        }
    }
    public static WebApplicationBuilder RegisterBookingServices(this WebApplicationBuilder builder)
    {
        ArgumentNullException.ThrowIfNull(builder);
        builder.Services.BindDbContext<BookingDbContext>();
        builder.Services.AddScoped<IDbInitializer, BookingDbInitializer>();
        builder.Services.AddKeyedScoped<IRepository<Customer>, BookingRepository<Customer>>("booking:customers");
        builder.Services.AddKeyedScoped<IReadRepository<Customer>, BookingRepository<Customer>>("booking:customers");
        builder.Services.AddKeyedScoped<IRepository<GroupMember>, BookingRepository<GroupMember>>("booking:groupmembers");
        builder.Services.AddKeyedScoped<IReadRepository<GroupMember>, BookingRepository<GroupMember>>("booking:groupmembers");
        builder.Services.AddKeyedScoped<IRepository<CourtRental>, BookingRepository<CourtRental>>("booking:courtrentals");
        builder.Services.AddKeyedScoped<IReadRepository<CourtRental>, BookingRepository<CourtRental>>("booking:courtrentals");
        builder.Services.AddKeyedScoped<IRepository<Group>, BookingRepository<Group>>("booking:groups");
        builder.Services.AddKeyedScoped<IReadRepository<Group>, BookingRepository<Group>>("booking:groups");
        return builder;
    }
    public static WebApplication UseBookingModule(this WebApplication app)
    {
        return app;
    }
}
