using FSH.Framework.Core.Domain;
using FSH.Framework.Core.Domain.Contracts;
using FSH.Starter.WebApi.Booking.Domain.Events;

namespace FSH.Starter.WebApi.Booking.Domain;
public class Customer : AuditableEntity, IAggregateRoot
{
    public string Name { get; private set; } = string.Empty;
    public string ClubNumber { get; private set; } = string.Empty;
    public string? Ssn { get; private set; }
    public string? Address { get; private set; }
    public string? Notes { get; private set; }
    public string? Email { get; private set; }
    public string? PhoneNumber { get; private set; }
    public string? PostalCode { get; private set; }
    public virtual ICollection<GroupMember> Groups { get; private set; } = new HashSet<GroupMember>();
    public virtual ICollection<CourtRentalShare> CourtRentalShares { get; private set; } = new HashSet<CourtRentalShare>();

    private Customer() { }

    private Customer(Guid id, string name, string clubNumber, string? ssn, string? address, string? notes, string? email, string? phoneNumber, string? postalCode)
    {
        Id = id;
        Name = name;
        ClubNumber = clubNumber;
        Ssn = ssn;
        Address = address;
        Notes = notes;
        Email = email;
        PhoneNumber = phoneNumber;
        PostalCode = postalCode;

        QueueDomainEvent(new CustomerCreated { Customer = this });
    }

    public static Customer Create(string name, string clubNumber, string? ssn, string? address, string? notes, string? email, string? phoneNumber, string? postalCode)
    {
        return new Customer(Guid.NewGuid(), name, clubNumber, ssn, address, notes, email, phoneNumber, postalCode);
    }

    public Customer Update(string? name, string? clubNumber, string? ssn, string? address, string? notes, string? email, string? phoneNumber, string? postalCode)
    {
        bool isUpdated = false;

        if (!string.IsNullOrWhiteSpace(name) && !string.Equals(Name, name, StringComparison.OrdinalIgnoreCase))
        {
            Name = name;
            isUpdated = true;
        }

        if (!string.IsNullOrWhiteSpace(clubNumber) && !string.Equals(ClubNumber, name, StringComparison.OrdinalIgnoreCase))
        {
            ClubNumber = clubNumber;
            isUpdated = true;
        }

        if (!string.Equals(Ssn, ssn, StringComparison.OrdinalIgnoreCase))
        {
            Ssn = ssn;
            isUpdated = true;
        }

        if (!string.Equals(Address, address, StringComparison.OrdinalIgnoreCase))
        {
            Address = address;
            isUpdated = true;
        }

        if (!string.Equals(Notes, notes, StringComparison.OrdinalIgnoreCase))
        {
            Notes = notes;
            isUpdated = true;
        }

        if (!string.Equals(Email, email, StringComparison.OrdinalIgnoreCase))
        {
            Email = email;
            isUpdated = true;
        }

        if(!string.Equals(PhoneNumber, phoneNumber, StringComparison.OrdinalIgnoreCase))
        {
            PhoneNumber = phoneNumber;
            isUpdated = true;
        }
        
        if(!string.Equals(PostalCode, postalCode, StringComparison.OrdinalIgnoreCase))
        {
            PostalCode = postalCode;
            isUpdated = true;
        }

        if (isUpdated)
        {
            QueueDomainEvent(new CustomerUpdated { Customer = this });
        }

        return this;
    }
}

