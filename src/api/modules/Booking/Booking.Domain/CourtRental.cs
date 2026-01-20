using FSH.Framework.Core.Domain;
using FSH.Framework.Core.Domain.Contracts;
using FSH.Starter.WebApi.Booking.Domain.Events;

namespace FSH.Starter.WebApi.Booking.Domain;
public partial class CourtRental : AuditableEntity, IAggregateRoot
{
    public DateTime StartDate { get; private set; }
    public TimeSpan StartTime { get; private set; }
    public DayOfWeek Weekday { get; private set; }
    public decimal Amount { get; private set; }
    public decimal Discount { get; private set; }
    public int Duration { get; private set; }
    public DateTime EndDate { get; private set; }
    public Guid? GroupId { get; private set; }
    public virtual Group Group { get; private set; } = default!;
    public int Court { get; private set; }
    
    public virtual ICollection<CourtRentalShare> CourtRentalShares { get; private set; } = new HashSet<CourtRentalShare>();
    public virtual ICollection<CourtRentalSession> CourtRentalSessions { get; private set; } = new HashSet<CourtRentalSession>();

    private CourtRental(Guid id, DateTime startDate, TimeSpan startTime, DateTime endDate, DayOfWeek weekday, decimal amount, decimal discount, int duration, int court, Guid? groupId)
    {
        Id = id;
        Court = court;
        Amount = amount;
        Discount = discount;
        StartTime = startTime;
        Duration = duration;
        StartDate = startDate;
        EndDate = endDate;
        Weekday = weekday;
        GroupId = groupId;

        QueueDomainEvent(new CourtRentalCreated { CourtRental = this });
    }

    public static CourtRental Create(DateTime startDate, TimeSpan startTime, DateTime endDate, DayOfWeek weekday, decimal amount, decimal discount, int duration, int court, Guid? groupId)
    {
        return new CourtRental(Guid.NewGuid(), startDate, startTime, endDate, weekday, amount, discount, duration, court, groupId);
    }

    public CourtRental Update(DateTime? startDate, TimeSpan? startTime, DateTime? endDate, DayOfWeek? weekday, decimal? amount, decimal? discount, int? duration, int court, Guid? groupId)
    {
        bool isUpdated = false;

        if (Court != court)
        {
            Court = court;
            isUpdated = true;
        }

        if (amount.HasValue && Amount != amount.Value)
        {
            Amount = amount.Value;
            isUpdated = true;
        }

        if (discount.HasValue && Discount != discount.Value)
        {
            Discount = discount.Value;
            isUpdated = true;
        }

        if (duration.HasValue && duration != Duration)
        {
            Duration = duration.Value;
            isUpdated = true;
        }

        if(weekday.HasValue && Weekday != weekday.Value)
        {
            Weekday = weekday.Value;
            isUpdated = true;
        }

        if (!string.Equals(StartDate, startDate))
        {
            StartDate = startDate ?? StartDate;
            isUpdated = true;
        }

        if(!string.Equals(EndDate, endDate))
        {
            EndDate = endDate ?? EndDate;
            isUpdated = true;
        }

        if (!string.Equals(StartTime, startTime))
        {
            StartTime = startTime ?? StartTime;
            isUpdated = true;
        }

        if (groupId.HasValue && groupId.Value != Guid.Empty && GroupId != groupId.Value)
        {
            GroupId = groupId.Value;
            isUpdated = true;
        }

        if (isUpdated)
        {
            QueueDomainEvent(new CourtRentalUpdated { CourtRental = this });
        }

        return this;
    }
}

