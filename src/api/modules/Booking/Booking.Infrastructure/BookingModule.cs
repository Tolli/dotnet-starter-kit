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
using Microsoft.Extensions.Hosting;

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
            //customerGroup.MapGetCustomerWithSharesEndpoint();
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
            groupMemberGroup.MapGetGroupMembersByGroupIdEndpoint();
            groupMemberGroup.MapGroupMemberUpdateEndpoint();
            groupMemberGroup.MapGroupMemberDeleteEndpoint();

            var courtRentalGroup = app.MapGroup("courtrentals").WithTags("courtrentals");
            courtRentalGroup.MapCourtRentalCreationEndpoint();
            courtRentalGroup.MapGetCourtRentalEndpoint();
            courtRentalGroup.MapGetCourtRentalsByGroupIdEndpoint();
            courtRentalGroup.MapGetCourtRentalListEndpoint();
            courtRentalGroup.MapGetCourtRentalListByHouseDayTimeCourtEndpoint();
            courtRentalGroup.MapCourtRentalUpdateEndpoint();
            courtRentalGroup.MapCourtRentalDeleteEndpoint();

            var courtrentalshareGroup = app.MapGroup("courtrentalshares").WithTags("courtrentalshares");
            courtrentalshareGroup.MapCourtRentalShareCreationEndpoint();
            courtrentalshareGroup.MapGetCourtRentalShareEndpoint();
            courtrentalshareGroup.MapGetCourtRentalSharesByCourtRentalIdEndpoint();
            courtrentalshareGroup.MapGetCourtRentalShareListEndpoint();
            courtrentalshareGroup.MapCourtRentalShareUpdateEndpoint();
            courtrentalshareGroup.MapCourtRentalShareDeleteEndpoint();

            var courtRentalSessionGroup = app.MapGroup("courtrentalsessions").WithTags("courtrentalsessions");
            courtRentalSessionGroup.MapCourtRentalSessionCreationEndpoint();
            courtRentalSessionGroup.MapGetCourtRentalSessionEndpoint();
            courtRentalSessionGroup.MapGetCourtRentalSessionsByCourtRentalIdEndpoint();
            courtRentalSessionGroup.MapGetCourtRentalSessionListEndpoint();
            courtRentalSessionGroup.MapGetCourtRentalSessionListByDateCourtEndpoint();
            courtRentalSessionGroup.MapCourtRentalSessionUpdateEndpoint();
            courtRentalSessionGroup.MapCourtRentalSessionDeleteEndpoint();

            var receiptGroup = app.MapGroup("receipts").WithTags("receipts");
            receiptGroup.MapReceiptCreationEndpoint();
            receiptGroup.MapGetReceiptEndpoint();
            receiptGroup.MapGetReceiptListEndpoint();
            receiptGroup.MapReceiptUpdateEndpoint();
            receiptGroup.MapReceiptDeleteEndpoint();

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
        builder.Services.AddKeyedScoped<IRepository<Receipt>, BookingRepository<Receipt>>("booking:receipts");
        builder.Services.AddKeyedScoped<IReadRepository<Receipt>, BookingRepository<Receipt>>("booking:receipts");
        builder.Services.AddKeyedScoped<IRepository<CourtRental>, BookingRepository<CourtRental>>("booking:courtrentals");
        builder.Services.AddKeyedScoped<IReadRepository<CourtRental>, BookingRepository<CourtRental>>("booking:courtrentals");
        builder.Services.AddKeyedScoped<IRepository<CourtRentalShare>, BookingRepository<CourtRentalShare>>("booking:courtrentalshares");
        builder.Services.AddKeyedScoped<IReadRepository<CourtRentalShare>, BookingRepository<CourtRentalShare>>("booking:courtrentalshares");
        builder.Services.AddKeyedScoped<IRepository<CourtRentalSession>, BookingRepository<CourtRentalSession>>("booking:courtrentalsessions");
        builder.Services.AddKeyedScoped<IReadRepository<CourtRentalSession>, BookingRepository<CourtRentalSession>>("booking:courtrentalsessions");
        builder.Services.AddKeyedScoped<IRepository<Group>, BookingRepository<Group>>("booking:groups");
        builder.Services.AddKeyedScoped<IReadRepository<Group>, BookingRepository<Group>>("booking:groups");
        return builder;
    }

    public static WebApplication UseBookingModule(this WebApplication app)
    {
        return app;
    }
}
