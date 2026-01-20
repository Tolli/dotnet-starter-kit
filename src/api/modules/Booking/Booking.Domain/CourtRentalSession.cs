using FSH.Framework.Core.Domain;
using FSH.Framework.Core.Domain.Contracts;
using FSH.Starter.WebApi.Booking.Domain.Events;

namespace FSH.Starter.WebApi.Booking.Domain;

public partial class CourtRentalSession : AuditableEntity, IAggregateRoot
{
    public DateTime StartDate { get; private set; }
    public DateTime EndDate { get; private set; }
    public Guid? CourtRentalId { get; private set; }
    public int Court { get; private set; }
    public virtual CourtRental CourtRental { get; private set; } = default!;
    private CourtRentalSession(Guid id, DateTime startDate, DateTime endDate, int court, Guid? courtRentalId)
    {
        Id = id;
        Court = court;
        StartDate = startDate;
        EndDate = endDate;
        CourtRentalId = courtRentalId;

        QueueDomainEvent(new CourtRentalSessionCreated { CourtRentalSession = this });
    }

    public static CourtRentalSession Create(DateTime startDate, DateTime endDate, int court, Guid? courtRentalId)
    {
        return new CourtRentalSession(Guid.NewGuid(), startDate, endDate, court, courtRentalId);
    }

    public CourtRentalSession Update(DateTime? startDate, DateTime? endDate, int court, Guid? courtRentalId)
    {
        bool isUpdated = false;

        if (Court != court)
        {
            Court = court;
            isUpdated = true;
        }

        if (!string.Equals(StartDate, startDate))
        {
            StartDate = startDate ?? StartDate;
            isUpdated = true;
        }

        if (!string.Equals(EndDate, endDate))
        {
            EndDate = endDate ?? EndDate;
            isUpdated = true;
        }

        if (courtRentalId.HasValue && courtRentalId.Value != Guid.Empty && CourtRentalId != courtRentalId.Value)
        {
            CourtRentalId = courtRentalId.Value;
            isUpdated = true;
        }

        if (isUpdated)
        {
            QueueDomainEvent(new CourtRentalSessionUpdated { CourtRentalSession = this });
        }

        return this;
    }
}

