using System.ComponentModel.DataAnnotations;

namespace GDC.EventHost.Shared.ValidationAttributes
{
    public class StatusIsValid : ValidationAttribute
    {
        private const string InvalidStatusMessage = "You should enter a valid status id.";

        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            // Rule: status must be non-null and a valid enum

            if (value is null)
            {
                return new ValidationResult(InvalidStatusMessage);
            }

            return Enum.IsDefined(typeof(Enums.StatusEnum), value)
                ? ValidationResult.Success
                : new ValidationResult(InvalidStatusMessage);  
        }
    }
}
