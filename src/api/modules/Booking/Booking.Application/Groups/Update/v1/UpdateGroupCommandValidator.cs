using FluentValidation;

namespace FSH.Starter.WebApi.Booking.Application.Groups.Update.v1;
public class UpdateGroupCommandValidator : AbstractValidator<UpdateGroupCommand>
{
    public UpdateGroupCommandValidator()
    {
        RuleFor(b => b.Name).NotEmpty().MinimumLength(2).MaximumLength(100);
        RuleFor(b => b.StartDate).NotEmpty();
        RuleFor(b => b.EndDate).NotEmpty().GreaterThan(b => b.StartDate);
    }
}
