using FluentValidation;

namespace FSH.Starter.WebApi.Booking.Application.CourtRentals.Create.v1;
public class CreateCourtRentalCommandValidator : AbstractValidator<CreateCourtRentalCommand>
{
    public CreateCourtRentalCommandValidator()
    {
        RuleFor(p => p.Name).NotEmpty().MinimumLength(2).MaximumLength(75);
        RuleFor(p => p.Price).GreaterThan(0);
    }
}
