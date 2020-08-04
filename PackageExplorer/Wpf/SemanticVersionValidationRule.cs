using System.Globalization;
using System.Windows.Controls;
using NuGet.Versioning;

namespace PackageExplorer
{
    public class SemanticVersionValidationRule : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            var stringValue = (string)value;
            if (string.IsNullOrEmpty(stringValue))
            {
                return ValidationResult.ValidResult;
            }

            if (stringValue.Contains("$", System.StringComparison.Ordinal))
            {
                return ValidationResult.ValidResult;
            }

            if (NuGetVersion.TryParse(stringValue, out _))
            {
                return ValidationResult.ValidResult;
            }
            else
            {
                return new ValidationResult(false, "版本格式不正确。有效版本的例子包括 '1.0', '2.0.1-alpha', '1.2.3.4-RC'.");
            }
        }
    }
}
