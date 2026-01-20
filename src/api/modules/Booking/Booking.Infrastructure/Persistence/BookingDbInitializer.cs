using FSH.Framework.Core.Persistence;
using FSH.Starter.WebApi.Booking.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace FSH.Starter.WebApi.Booking.Infrastructure.Persistence;
internal sealed class BookingDbInitializer(
    ILogger<BookingDbInitializer> logger,
    BookingDbContext context) : IDbInitializer
{
    public async Task MigrateAsync(CancellationToken cancellationToken)
    {
        if ((await context.Database.GetPendingMigrationsAsync(cancellationToken)).Any())
        {
            await context.Database.MigrateAsync(cancellationToken).ConfigureAwait(false);
            logger.LogInformation("[{Tenant}] applied database migrations for booking module", context.TenantInfo!.Identifier);
        }
    }

    public async Task SeedAsync(CancellationToken cancellationToken)
    {

        try
        {
            //var cust = await context.Customers.FirstOrDefaultAsync(t => t.Name == "Þórhallur Einisson", cancellationToken).ConfigureAwait(false);
            //if (cust is null)
            //{
            //    const string Name = "Þórhallur Einisson";
            //    const string Ssn = "1906733329";
            //    const string ClubNumber = "F1234";
            //    const string Address = "Laufskógar 34";
            //    const string Notes = "Some notes about Tolli";
            //    const string Email = "tolli@tolli.com";
            //    const string PhoneNumber = "615-0099";
            //    const string PostalCode = "810";
            //    var customer = Customer.Create(Name, ClubNumber, Ssn, Address, Notes, Email, PhoneNumber, PostalCode);
            //    await context.Customers.AddAsync(customer, cancellationToken);
            //    await context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
            //    logger.LogInformation("[{Tenant}] seeding default booking data", context.TenantInfo!.Identifier);
            //    cust = customer;
            //}

            //var cust2 = await context.Customers.FirstOrDefaultAsync(t => t.Name == "Hrund Guðmundsdóttir", cancellationToken).ConfigureAwait(false);
            //if (cust2 is null)
            //{
            //    const string Name = "Hrund Guðmundsdóttir";
            //    const string Ssn = "1604743359";
            //    const string ClubNumber = "F5678";
            //    const string Address = "Laufskógar 34";
            //    const string Notes = "Some notes about Hrund";
            //    const string Email = "hrund@hrund.com";
            //    const string PhoneNumber = "615-0033";
            //    const string PostalCode = "810";

            //    var customer = Customer.Create(Name, ClubNumber, Ssn, Address, Notes, Email, PhoneNumber, PostalCode);
            //    await context.Customers.AddAsync(customer, cancellationToken);
            //    await context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
            //    logger.LogInformation("[{Tenant}] seeding default booking data", context.TenantInfo!.Identifier);
            //    cust2 = customer;
            //}

            //var cust3 = await context.Customers.FirstOrDefaultAsync(t => t.Name == "Úlfur Þórhallsson", cancellationToken).ConfigureAwait(false);
            //if (cust3 is null)
            //{
            //    const string Name = "Úlfur Þórhallsson";
            //    const string SSN = "0201082430";
            //    const string ClubNumber = "F91011";
            //    const string Address = "Laufskógar 34";
            //    const string Notes = "Some notes about Úlfur";
            //    const string Email = "ulfurx@gmail.com";
            //    const string PhoneNumber = "768-0090";
            //    const string PostalCode = "810";

            //    var customer = Customer.Create(Name, ClubNumber, SSN, Address, Notes, Email, PhoneNumber, PostalCode);
            //    await context.Customers.AddAsync(customer, cancellationToken);
            //    await context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
            //    logger.LogInformation("[{Tenant}] seeding default booking data", context.TenantInfo!.Identifier);
            //    cust3 = customer;
            //}

            //var cust4 = await context.Customers.FirstOrDefaultAsync(t => t.Name == "Íris Þórhallsdóttir", cancellationToken).ConfigureAwait(false);
            //if (cust4 is null)
            //{
            //    const string Name = "Íris Þórhallsdóttir";
            //    const string Ssn = "2008132430";
            //    const string ClubNumber = "F121314";
            //    const string Address = "Laufskógar 34";
            //    const string Notes = "Some notes about Íris";
            //    const string Email = "irispiris@gmail.com";
            //    const string PhoneNumber = "690-0733";
            //    const string PostalCode = "810";

            //    var customer = Customer.Create(Name, ClubNumber, Ssn, Address, Notes, Email, PhoneNumber, PostalCode);
            //    await context.Customers.AddAsync(customer, cancellationToken);
            //    await context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
            //    logger.LogInformation("[{Tenant}] seeding default booking data", context.TenantInfo!.Identifier);
            //    cust4 = customer;
            //}

            //var grp = await context.Groups.FirstOrDefaultAsync(t => t.Name == "Öllarar", cancellationToken).ConfigureAwait(false);
            //if (grp is null)
            //{
            //    var name = "Öllarar";
            //    var startDate = new DateTime(2025, 09, 30);
            //    var endDate = new DateTime(2026, 05, 31);
            //    var group = Group.Create(name, startDate, endDate);
            //    await context.Groups.AddAsync(group, cancellationToken);
            //    await context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
            //    logger.LogInformation("[{Tenant}] seeding default booking data", context.TenantInfo!.Identifier);
            //    grp = group;
            //}
            //var member = await context.GroupMembers.FirstOrDefaultAsync(gm => gm.GroupId == grp.Id && gm.CustomerId == cust.Id, cancellationToken).ConfigureAwait(false);
            //if (member is null)
            //{
            //    var price = 10000;
            //    var percentage = 10;
            //    var isContact = true;
            //    var groupMember = GroupMember.Create(grp.Id, cust.Id, price, percentage, isContact);
            //    await context.GroupMembers.AddAsync(groupMember, cancellationToken);
            //    await context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
            //    logger.LogInformation("[{Tenant}] seeding default booking data", context.TenantInfo!.Identifier);
            //    member = groupMember;
            //}

            //var member2 = await context.GroupMembers.FirstOrDefaultAsync(gm => gm.GroupId == grp.Id && gm.CustomerId == cust2.Id, cancellationToken).ConfigureAwait(false);
            //if (member2 is null)
            //{
            //    var price = 25000;
            //    var percentage = 25;
            //    var isContact = true;
            //    var groupMember = GroupMember.Create(grp.Id, cust2.Id, price, percentage, isContact);
            //    await context.GroupMembers.AddAsync(groupMember, cancellationToken);
            //    await context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
            //    logger.LogInformation("[{Tenant}] seeding default booking data", context.TenantInfo!.Identifier);
            //    member2 = groupMember;
            //}

            //var courtrental = await context.CourtRentals.FirstOrDefaultAsync(cr => cr.Court == 1 && cr.StartTime == new TimeSpan(20,10,00) && cr.Weekday == DayOfWeek.Monday && cr.GroupId == grp.Id && cr.StartDate == new DateTime(2025,09,01), cancellationToken).ConfigureAwait(false);
            //if (courtrental is null)
            //{
            //    var amount = 240000;
            //    var discount = 1000;
            //    var startTime = new TimeSpan(20, 10, 00);
            //    var duration = 50;
            //    var court = 1;
            //    var weekday = DayOfWeek.Monday;
            //    var startDate = new DateTime(2025, 09, 01);
            //    var endDate = new DateTime(2026, 05, 31);
            //    courtrental = CourtRental.Create(startDate, startTime, endDate, weekday, amount, discount, duration, court, grp.Id);
            //    await context.CourtRentals.AddAsync(courtrental, cancellationToken);
            //    await context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
            //    logger.LogInformation("[{Tenant}] seeding default booking data", context.TenantInfo!.Identifier);
            //}

            //var courtrental2 = await context.CourtRentals.FirstOrDefaultAsync(cr => cr.Court == 1 && cr.StartTime == new TimeSpan(20, 10, 00) && cr.Weekday == DayOfWeek.Thursday && cr.GroupId == grp.Id && cr.StartDate == new DateTime(2025, 09, 01), cancellationToken).ConfigureAwait(false);
            //if (courtrental2 is null)
            //{
            //    var amount = 240000;
            //    var discount = 1000;
            //    var startTime = new TimeSpan(20, 10, 00);
            //    int duration = 50;
            //    var weekday = DayOfWeek.Thursday;
            //    var court = 1;
            //    var startDate = new DateTime(2025, 09, 01);
            //    var endDate = new DateTime(2026, 05, 31);
            //    courtrental2 = CourtRental.Create(startDate, startTime, endDate, weekday, amount, discount, duration, court, grp.Id);
            //    await context.CourtRentals.AddAsync(courtrental2, cancellationToken);
            //    await context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
            //    logger.LogInformation("[{Tenant}] seeding default booking data", context.TenantInfo!.Identifier);
            //}

            //var courtrentalshare1 = await context.CourtRentalShares.FirstOrDefaultAsync(crs => crs.CourtRentalId == courtrental.Id && crs.CustomerId == cust.Id, cancellationToken).ConfigureAwait(false);
            //if (courtrentalshare1 is null)
            //{
            //    var courtRentalShare = CourtRentalShare.Create(courtrental.Id, cust.Id, 1000,100,10,DateTime.Now, DateTime.Now, "");
            //    await context.CourtRentalShares.AddAsync(courtRentalShare, cancellationToken);
            //    await context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
            //    logger.LogInformation("[{Tenant}] seeding default booking data", context.TenantInfo!.Identifier);
            //}

            //var courtrentalshare2 = await context.CourtRentalShares.FirstOrDefaultAsync(crs => crs.CourtRentalId == courtrental.Id && crs.CustomerId == cust2.Id, cancellationToken).ConfigureAwait(false);
            //if (courtrentalshare2 is null)
            //{
            //    var courtRentalShare = CourtRentalShare.Create(courtrental.Id, cust2.Id, 1000, 100, 10, DateTime.Now, DateTime.Now, "");
            //    await context.CourtRentalShares.AddAsync(courtRentalShare, cancellationToken);
            //    await context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
            //    logger.LogInformation("[{Tenant}] seeding default booking data", context.TenantInfo!.Identifier);
            //}

            //var courtrentalshare3 = await context.CourtRentalShares.FirstOrDefaultAsync(crs => crs.CourtRentalId == courtrental.Id && crs.CustomerId == cust3.Id, cancellationToken).ConfigureAwait(false);
            //if (courtrentalshare3 is null)
            //{
            //    var courtRentalShare = CourtRentalShare.Create(courtrental.Id, cust3.Id, 1000, 100, 10, DateTime.Now, DateTime.Now, "");
            //    await context.CourtRentalShares.AddAsync(courtRentalShare, cancellationToken);
            //    await context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
            //    logger.LogInformation("[{Tenant}] seeding default booking data", context.TenantInfo!.Identifier);
            //}

            //var courtrentalshare4 = await context.CourtRentalShares.FirstOrDefaultAsync(crs => crs.CourtRentalId == courtrental.Id && crs.CustomerId == cust4.Id, cancellationToken).ConfigureAwait(false);
            //if (courtrentalshare4 is null)
            //{
            //    var courtRentalShare = CourtRentalShare.Create(courtrental.Id, cust4.Id, 1000, 100, 10, DateTime.Now, DateTime.Now, "");
            //    await context.CourtRentalShares.AddAsync(courtRentalShare, cancellationToken);
            //    await context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
            //    logger.LogInformation("[{Tenant}] seeding default booking data", context.TenantInfo!.Identifier);
            //}
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "[{Tenant}] an error occurred while seeding default booking data", context.TenantInfo!.Identifier);
            throw;
        }
    }
}
