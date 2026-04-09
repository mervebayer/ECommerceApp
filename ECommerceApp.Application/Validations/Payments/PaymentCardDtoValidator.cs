using ECommerceApp.Application.DTOs.Payments;
using FluentValidation;
using System.Globalization;

namespace ECommerceApp.Application.Validations.Payments
{
    public class PaymentCardDtoValidator : AbstractValidator<PaymentCardDto>
    {
        public PaymentCardDtoValidator()
        {
            RuleFor(x => x.CardHolderName)
                .NotEmpty()
                .MaximumLength(200)
                .WithMessage("Card holder name is required.");

            RuleFor(x => x.CardNumber)
                .NotEmpty()
                .Matches(@"^\d{12,19}$")
                .WithMessage("Card number must contain 12 to 19 digits.");

            RuleFor(x => x.ExpireMonth)
                .NotEmpty()
                .Matches(@"^(0[1-9]|1[0-2])$")
                .WithMessage("Expire month must be between 01 and 12.");

            RuleFor(x => x.ExpireYear)
                .NotEmpty()
                .Matches(@"^\d{2}(\d{2})?$")
                .WithMessage("Expire year must be 2 or 4 digits.");

            RuleFor(x => x.Cvc)
                .NotEmpty()
                .Matches(@"^\d{3,4}$")
                .WithMessage("CVC must be 3 or 4 digits.");

            RuleFor(x => x)
                .Must(HaveNonExpiredCard)
                .WithMessage("Card expiry date cannot be in the past.");
        }

        private static bool HaveNonExpiredCard(PaymentCardDto card)
        {
            if (string.IsNullOrWhiteSpace(card.ExpireMonth) || string.IsNullOrWhiteSpace(card.ExpireYear))
                return true;

            if (!int.TryParse(card.ExpireMonth, NumberStyles.None, CultureInfo.InvariantCulture, out var month))
                return true;

            if (month is < 1 or > 12)
                return true;

            if (!TryParseYear(card.ExpireYear, out var year))
                return true;

            var now = DateTime.UtcNow;
            return year > now.Year || (year == now.Year && month >= now.Month);
        }

        private static bool TryParseYear(string expireYear, out int year)
        {
            if (!int.TryParse(expireYear, NumberStyles.None, CultureInfo.InvariantCulture, out year))
                return false;

            if (expireYear.Length == 2)
            {
                year += 2000;
            }

            return expireYear.Length is 2 or 4;
        }
    }
}
