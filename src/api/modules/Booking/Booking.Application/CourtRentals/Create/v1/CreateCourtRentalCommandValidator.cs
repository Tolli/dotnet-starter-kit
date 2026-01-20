using FluentValidation;

namespace FSH.Starter.WebApi.Booking.Application.CourtRentals.Create.v1;
public class CreateCourtRentalCommandValidator : AbstractValidator<CreateCourtRentalCommand>
{
    public CreateCourtRentalCommandValidator()
    {
        RuleFor(p => p.Amount).GreaterThan(0);
    }
}
