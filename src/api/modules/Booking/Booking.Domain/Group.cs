using System.Text.RegularExpressions;
using FSH.Framework.Core.Domain;
using FSH.Framework.Core.Domain.Contracts;
using FSH.Starter.WebApi.Booking.Domain.Events;

namespace FSH.Starter.WebApi.Booking.Domain;
public class Group : AuditableEntity, IAggregateRoot
{
    public string Name { get; private set; } = string.Empty;
    public DateTime StartDate { get; private set; }

    public DateTime EndDate { get; private set; }

    public virtual ICollection<GroupMember>? Members { get; private set; } = new HashSet<GroupMember>();

    public virtual ICollection<CourtRental> CourtRentals { get; private set; } = new HashSet<CourtRental>();

    private Group() { }

    private Group(Guid id, string name, DateTime startDate, DateTime endDate)
    {
        Id = id;
        Name = name;
        StartDate = startDate;
        EndDate = endDate;
        QueueDomainEvent(new GroupCreated { Group = this });
    }

    public static Group Create(string name, DateTime startDate, DateTime endDate)
    {
        return new Group(Guid.NewGuid(), name, startDate, endDate);
    }

    public Group Update(string? name, DateTime? startDate, DateTime? endDate, Guid? contactId)
    {
        bool isUpdated = false;

        if (!string.IsNullOrWhiteSpace(name) && !string.Equals(Name, name, StringComparison.OrdinalIgnoreCase))
        {
            Name = name;
            isUpdated = true;
        }

        if (startDate != null && StartDate != startDate.Value)
        {
            StartDate = startDate.Value;
            isUpdated = true;
        }

        if (endDate != null && EndDate != endDate.Value)
        {
            EndDate = endDate.Value;
            isUpdated = true;
        }

        if (isUpdated)
        {
            QueueDomainEvent(new GroupUpdated { Group = this });
        }

        return this;
    }
}


