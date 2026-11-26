using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace StudentManagementAPI.DTOs
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter)]
    public sealed class IndianMobilePhoneAttribute : ValidationAttribute
    {
        private static readonly Regex PhonePattern = new(
            @"^(?:\+91)?[6-9][0-9]{9}\z",
            RegexOptions.CultureInvariant | RegexOptions.Compiled);

        public override bool IsValid(object? value)
        {
            // Null represents the optional phone field. Empty strings and formatted numbers are invalid.
            return value is null || value is string phone && PhonePattern.IsMatch(phone);
        }
    }

    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter)]
    public sealed class NotFutureDateAttribute : ValidationAttribute
    {
        public override bool IsValid(object? value)
        {
            return value is null || value is DateTime date && date.Date <= DateTime.Today;
        }
    }
}
