using ECommerceApp.Application.DTOs.Payments;
using FluentValidation;

namespace ECommerceApp.Application.Validations.Payments
{
    public class PayOrderRequestDtoValidator : AbstractValidator<PayOrderRequestDto>
    {
        public PayOrderRequestDtoValidator()
        {
            RuleFor(x => x.PaymentCard)
                .NotNull()
                .WithMessage("Payment card information is required.");

            When(x => x.PaymentCard is not null, () =>
            {
                RuleFor(x => x.PaymentCard!)
                    .SetValidator(new PaymentCardDtoValidator());
            });

            RuleFor(x => x.IdempotencyKey)
                .NotEmpty()
                .MaximumLength(100);

        }
    }
}
