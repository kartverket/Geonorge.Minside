﻿namespace Geonorge.MinSide.Models
{
    public class ApplicationSettings
    {
        public string BuildVersionNumber { get; set; }
        public string EnvironmentName { get; set; }
        public string ProxyAddress { get; set; }
        public Urls Urls { get; set; }
        public string BaatAuthzApiCredentials { get; set; }
        public string RedirectUri { get; set; }
        public string PostLogoutRedirectUri { get; set; }
        public string FilePath { get; set; }
        public string SmtpHost { get; set; }
        public string WebmasterEmail { get; set; }
        public string LogApi { get; set; }
        public string LogApiKey { get; set; }    
        public string UrlProxy { get; set; }
    }

    public class Urls
    {
        public string GeonorgeRoot { get; set; }
        public string BaatAuthzApi { get; set; }
    }
}
