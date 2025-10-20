using FluentValidation;

namespace FSH.Starter.WebApi.Booking.Application.GroupMembers.Create.v1;
public class CreateGroupMemberCommandValidator : AbstractValidator<CreateGroupMemberCommand>
{
    public CreateGroupMemberCommandValidator()
    {
        RuleFor(p => p.GroupId).NotEmpty();
        RuleFor(p => p.CustomerId).NotEmpty();
        RuleFor(p => p.Price).GreaterThan(0);
        RuleFor(p => p.Percentage).GreaterThan(0);
        RuleFor(p => p.IsContact).NotNull();
    }
}
