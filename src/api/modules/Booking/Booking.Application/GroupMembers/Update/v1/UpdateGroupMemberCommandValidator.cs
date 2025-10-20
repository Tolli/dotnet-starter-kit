using FluentValidation;

namespace FSH.Starter.WebApi.Booking.Application.GroupMembers.Update.v1;
public class UpdateGroupMemberCommandValidator : AbstractValidator<UpdateGroupMemberCommand>
{
    public UpdateGroupMemberCommandValidator()
    {
        RuleFor(p => p.Id).NotEmpty();
        RuleFor(p => p.GroupId).NotEmpty();
        RuleFor(p => p.CustomerId).NotEmpty();
        RuleFor(p => p.Price).GreaterThan(0);
        RuleFor(p => p.Percentage).InclusiveBetween(0, 100);
        RuleFor(p => p.IsContact).NotNull();
    }
}
