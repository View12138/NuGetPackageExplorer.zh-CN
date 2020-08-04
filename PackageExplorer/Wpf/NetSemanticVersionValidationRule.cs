using System;
using System.Globalization;
using System.Windows.Controls;

namespace PackageExplorer
{
    public class NetSemanticVersionValidationRule : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            var stringValue = (string)value;
            if (string.IsNullOrEmpty(stringValue))
            {
                return ValidationResult.ValidResult;
            }

            if (stringValue.Contains("$", StringComparison.Ordinal))
            {
                return ValidationResult.ValidResult;
            }

            if (Version.TryParse(stringValue, out _))
            {
                return ValidationResult.ValidResult;
            }
            else
            {
                return new ValidationResult(false, "版本格式不正确。有效版本的例子包括 '1.0', '2.0.1', '1.2.3.4'.");
            }
        }
    }
}
