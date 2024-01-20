using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace TemplateWorkApp
{
    internal static  class Validation
    {
        public static bool IsValidEmail(string email)
        {
            string pattern = @"^\S+@\S+\.\S+$";
            return Regex.IsMatch(email, pattern);
        }

        public static bool IsValidString(string someString)
        {
            return String.IsNullOrEmpty(someString);
        }
        public static bool BarTemplateListValid(List<string> barTemplate)
        {
            if (barTemplate.Count==0)
            {
                return false;
            }
            return true;
        }
    }
}
