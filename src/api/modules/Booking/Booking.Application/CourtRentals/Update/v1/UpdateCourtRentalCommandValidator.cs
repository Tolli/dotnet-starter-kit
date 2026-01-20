using FluentValidation;

namespace FSH.Starter.WebApi.Booking.Application.CourtRentals.Update.v1;
public class UpdateCourtRentalCommandValidator : AbstractValidator<UpdateCourtRentalCommand>
{
    public UpdateCourtRentalCommandValidator()
    {
        RuleFor(p => p.Amount).GreaterThan(0);
    }
}
