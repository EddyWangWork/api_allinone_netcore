using System.ComponentModel.DataAnnotations;

namespace Allinone.Domain
{
    public class NotMinDateAttribute : ValidationAttribute
    {
        public override bool IsValid(object? value)
        {
            if (value is DateTime date)
            {
                return date != DateTime.MinValue;
            }

            // Allow null values; use [Required] separately if needed
            return false;
        }
    }

    public class DateEarlierThanAttribute(string _comparisonProperty) : ValidationAttribute
    {
        //private readonly string _comparisonProperty;

        //public DateEarlierThanAttribute(string comparisonProperty)
        //{
        //    _comparisonProperty = comparisonProperty;
        //}

        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            var currentValue = value as DateTime?;

            var property = validationContext.ObjectType.GetProperty(_comparisonProperty);
            if (property == null)
                return new ValidationResult($"Unknown property: {_comparisonProperty}");

            var comparisonValue = property.GetValue(validationContext.ObjectInstance) as DateTime?;

            if (currentValue.HasValue && comparisonValue.HasValue && currentValue > comparisonValue)
            {
                return new ValidationResult(ErrorMessage ?? $"{validationContext.MemberName} must be earlier than {_comparisonProperty}.");
            }

            return ValidationResult.Success;
        }
    }
}
