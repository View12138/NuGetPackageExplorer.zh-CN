using System;
using System.Globalization;
using System.Windows.Controls;

namespace PackageExplorer
{
    public class PublishUrlValidationRule : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            var stringValue = (string)value;
            if (Uri.TryCreate(stringValue, UriKind.Absolute, out var url))
            {
                if (url!.Scheme.Equals(Uri.UriSchemeHttp, StringComparison.OrdinalIgnoreCase) ||
                    url.Scheme.Equals(Uri.UriSchemeHttps, StringComparison.OrdinalIgnoreCase))
                {
                    return ValidationResult.ValidResult;
                }
                else
                {
                    return new ValidationResult(false, "发布 Url 必须是 HTTP 或 HTTPS 地址。");
                }
            }
            else
            {
                return new ValidationResult(false, "无效的发布 Url。");
            }
        }
    }
}
