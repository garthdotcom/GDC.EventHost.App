using System.ComponentModel.DataAnnotations;

namespace GDC.EventHost.Shared.ValidationAttributes
{
    public class DateIsFutureOrNull : ValidationAttribute
    {
        private const string InvalidDateMessage = "You should enter a valid date.";
        private const string PastDateMessage = "You should enter a date that is in the future.";

        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            // Rule: value can be null, but if present it must be a valid future date.

            if (value is null)
            {
                return ValidationResult.Success;
            }

            bool dateIsValid = DateTime.TryParse(value.ToString(), out DateTime parsedDate);

            if (!dateIsValid)
            {
                return new ValidationResult(InvalidDateMessage);
            }
            else
            {
                if (parsedDate < DateTime.Now)
                {
                    return new ValidationResult(PastDateMessage);
                }
            }

            return ValidationResult.Success;
        }
    }
}