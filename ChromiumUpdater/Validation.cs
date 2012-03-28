using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;

namespace ChromiumUpdater
{
    public class UrlPlaceHolderValidationRule : ValidationRule
    {
        public override ValidationResult Validate(object value, System.Globalization.CultureInfo cultureInfo)
        {
            string url = value.ToString();

            if(String.IsNullOrEmpty(url) || String.IsNullOrWhiteSpace(url))
                return new ValidationResult(false, "Not valid url");

            if(!url.Contains("{0}"))
                return new ValidationResult(false, "Url must contains {0}!");

            return new ValidationResult(true, null);
        }
    }
}
