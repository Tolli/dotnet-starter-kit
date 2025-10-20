using FluentValidation;

namespace FSH.Starter.WebApi.Booking.Application.Groups.Create.v1;
public class CreateGroupCommandValidator : AbstractValidator<CreateGroupCommand>
{
    public CreateGroupCommandValidator()
    {
        RuleFor(b => b.Name).NotEmpty().MinimumLength(2).MaximumLength(100);
        RuleFor(b => b.StartDate).NotEmpty();
        RuleFor(b => b.EndDate).NotEmpty().GreaterThan(b => b.StartDate);
    }
}
