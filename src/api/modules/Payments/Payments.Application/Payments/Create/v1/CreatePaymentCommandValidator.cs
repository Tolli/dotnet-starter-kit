using FluentValidation;

namespace FSH.Starter.WebApi.Payments.Application.Payments.Create.v1;

public sealed class CreatePaymentCommandValidator : AbstractValidator<CreatePaymentCommand>
{
    public CreatePaymentCommandValidator()
    {
        RuleFor(p => p.CustomerId)
            .NotEmpty().WithMessage("Customer ID is required.");

        RuleFor(p => p.Amount)
            .GreaterThan(0).WithMessage("Amount must be greater than zero.");

        RuleFor(p => p.Currency)
            .NotEmpty().WithMessage("Currency is required.")
            .MaximumLength(3).WithMessage("Currency must be 3 characters.");

        RuleFor(p => p.Method)
            .IsInEnum().WithMessage("Invalid payment method.");
    }
}
