using System.ComponentModel.DataAnnotations;

namespace GDC.EventHost.Shared.ValidationAttributes
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter)]
    public class DateRangeIsValid : ValidationAttribute
    {
        private readonly string _comparisonProperty;
        private const string InvalidDateMessage = "You should enter a valid date.";
        private const string InvalidDateRangeMessage = "You should enter a valid date range.";

        public DateRangeIsValid(string comparisonProperty)
        {
            _comparisonProperty = comparisonProperty;
        }

        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            // Rules:
            // both dates can be null.
            // if start date is valid, end date can be null.
            // if both dates are valid, end date must be later than start date.

            // Usage error: required property was not supplied
            var property = validationContext.ObjectType.GetProperty(_comparisonProperty) ?? 
                throw new ArgumentException("Comparison property with the given name was not found");

            // first-value      second-value        result
            // -----------      ------------        ------
            // null             null                pass
            // null             x                   fail
            // bad-date         x                   fail
            // date             null                pass
            // x                bad-date            fail
            // date             date                check

            var firstValue = property.GetValue(validationContext.ObjectInstance);

            // Case 1: both values are null
            if (firstValue is null && value is null)
            {
                return ValidationResult.Success;
            }

            // Case 2: first value is null
            if (firstValue is null)
            {
                return new ValidationResult(InvalidDateRangeMessage);
            }

            // Case 3: first value is not a date
            if (!DateTime.TryParse(firstValue.ToString(), out DateTime _))
            {
                return new ValidationResult(InvalidDateMessage);
            }

            // Case 4: first value is date, second value is null
            if (value is null)
            {
                return ValidationResult.Success;
            }

            var secondValue = (IComparable)value;

            // Case 5: second value is not a date
            if (!DateTime.TryParse(secondValue.ToString(), out DateTime _))
            {
                return new ValidationResult(InvalidDateMessage);
            }

            // Case 6: second date value must be greater than first date value
            if (secondValue.CompareTo((IComparable)firstValue) < 0)
            {
                return new ValidationResult(InvalidDateRangeMessage);
            }

            return ValidationResult.Success;
        }
    }
}
