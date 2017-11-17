using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PoeSimCraftImporter
{
    public class Util
    {
        public static string Format(string text)
        {
            // Rules

            // Underscores are replaced by spaces
            // However trailing underscores represent seperate values
            var trailingUnderscoreCount = text.Reverse().TakeWhile(x => x == '_').Count();

            text = text.Replace("_", " ");

            // Add spaces before capitalized letters or digits
            text = AddSpaces(text);

            text = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(text);

            // Add spaces before capitalized letters or digits
            text = AddSpaces(text);

            // Remove any extra white space that snuck in
            while (text.Contains("  "))
            {
                text = text.Replace("  ", " ");
            }

            // Remove all leading and trailing white space
            text = text.Trim();

            // Remove white space after hyphin
            text = text.Replace("- ", "-");

            // All single quotes are duplicated (Sql formatting)
            text = text.Replace("'", "''");

            // Remove any extra single quotes that snuck in
            while (text.Contains("'''"))
            {
                text = text.Replace("'''", "''");
            }

            text = text.Replace(" +", "+");
            text = text.Replace(" %", "%");

            for (int i = 0; i < trailingUnderscoreCount; i++)
            {
                text = text + '_';
            }

            return "'" + text + "'";
        }

        private static string AddSpaces(string x)
        {
            if (string.IsNullOrEmpty(x))
            {
                return string.Empty;
            }

            return string.Concat(x.Select((y, i) => ShouldCapitalize(x, i) ? " " + y : y.ToString())).TrimStart(' ');
        }

        private static bool ShouldCapitalize(string s, int index)
        {
            bool wasDigit = false;
            if (index > 0)
            {
                wasDigit = Char.IsDigit(s[index - 1]);
            }

            return Char.IsUpper(s[index]) || (!wasDigit && Char.IsDigit(s[index]));
        }
    }
}
