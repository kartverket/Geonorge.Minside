using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Geonorge.MinSide.Utils
{
    public static class Helper
    {
        public static string CreateFileName(string fileExtension, string name, DateTime date, string organizationName)
        {
            name = FriendlyString(name);
            organizationName = FriendlyString(organizationName);
            string dateString = date.Day.ToString("00") + date.Month.ToString("00") + date.Year.ToString("0000");

            string filename = $"{name}_{dateString}_{organizationName}.{fileExtension}";

            return filename;
        }

        public static string FriendlyString(string value)
        {
            return Regex.Replace(value, @"[^A-Za-z0-9_\.~]+", "-");
        }
    }
}
