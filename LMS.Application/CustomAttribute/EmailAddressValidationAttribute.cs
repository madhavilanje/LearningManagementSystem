using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace LMS.Application.CustomAttribute
{
    public class EmailAddressValidationAttribute : ValidationAttribute
    {
        private const string emailRegexPattern = @"\A(?:[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?)\Z";

        public override bool IsValid(object value)
        {
            string emailAddress = value as string;

            if (string.IsNullOrEmpty(emailAddress))
                return false;

            return Regex.IsMatch(emailAddress, emailRegexPattern, RegexOptions.IgnoreCase);
        }
    }
}
