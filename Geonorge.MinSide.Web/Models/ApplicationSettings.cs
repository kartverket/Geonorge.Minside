namespace Geonorge.MinSide.Models
{
    public class ApplicationSettings
    {
        public string BuildVersionNumber { get; set; }
        public string EnvironmentName { get; set; }
        public Urls Urls { get; set; }
    }

    public class Urls
    {
        public string GeonorgeRoot { get; set; }
    }
}