using FluentValidation;

namespace FSH.Starter.WebApi.Booking.Application.Receipts.Update.v1;
public class UpdateReceiptCommandValidator : AbstractValidator<UpdateReceiptCommand>
{
    public UpdateReceiptCommandValidator()
    {
        RuleFor(p => p.Id).NotEmpty();
        RuleFor(p => p.GroupMemberId).NotEmpty();
    }
}
