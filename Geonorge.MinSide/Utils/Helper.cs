using System;
using System.Collections.Generic;
using System.IO;
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

            string filename = $"{name}_{dateString}_{organizationName}{fileExtension}";

            return filename;
        }

        public static string FriendlyString(string value)
        {
            return Regex.Replace(value, @"[^A-Za-z0-9_\.~]+", "-");
        }

        public static string GetFileExtension(string FileName)
        {
            return Path.GetExtension(FileName).ToLowerInvariant();
        }

        public static string[] PermittedFileExtensions = { ".pdf", ".doc", ".xls", ".docx", ".xlsx" };

        public static string GetContentType(string path)
        {
            var types = GetMimeTypes();
            var ext = Path.GetExtension(path).ToLowerInvariant();
            return types[ext];
        }

        private static Dictionary<string, string> GetMimeTypes()
        {
            return new Dictionary<string, string>
            {
                {".pdf", "application/pdf"},
                {".doc", "application/vnd.ms-word"},
                {".docx", "application/vnd.ms-word"},
                {".xls", "application/vnd.ms-excel"},
                {".xlsx", "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"},  
            };
        }

    }
}
