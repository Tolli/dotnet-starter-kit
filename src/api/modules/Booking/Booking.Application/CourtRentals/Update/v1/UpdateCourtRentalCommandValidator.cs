using FluentValidation;

namespace FSH.Starter.WebApi.Booking.Application.CourtRentals.Update.v1;
public class UpdateCourtRentalCommandValidator : AbstractValidator<UpdateCourtRentalCommand>
{
    public UpdateCourtRentalCommandValidator()
    {
        RuleFor(p => p.Name).NotEmpty().MinimumLength(2).MaximumLength(75);
        RuleFor(p => p.Price).GreaterThan(0);
    }
}
