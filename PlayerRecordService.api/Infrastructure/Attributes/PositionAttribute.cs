
using System.ComponentModel.DataAnnotations;

namespace PlayerRecordService.api.Infrastructure.Attributes
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter, AllowMultiple = false)]
    public class PositionAttribute : ValidationAttribute
    {
        private readonly string _expectedPattern = @"^\((-?\d+)(,-?\d+)*\)$";

        public override bool IsValid(object value)
        {
            if (value == null)
                return true; // Null values are considered valid. You can change this behavior if needed.

            string strValue = value as string;

            // Check if the string matches the expected pattern
            return strValue != null && System.Text.RegularExpressions.Regex.IsMatch(strValue, _expectedPattern);
        }
    }
}
