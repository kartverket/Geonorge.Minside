using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Geonorge.MinSide.Models
{
    public static class CodeList
    {
        public static Dictionary<string, string> DocumentStatus = new Dictionary<string, string>
        {
            {"Forslag", "Forslag"},
            {"Gyldig", "Gyldig"},
            {"Utgått", "Utgått"}
        };

        public static Dictionary<string, string> DocumentTypes = new Dictionary<string, string>
        {
            {"Avtaledokument", "Avtaledokument"},
            {"Bilag 1", "Bilag 1"},
            {"Bilag 2", "Bilag 2"},
            {"Bilag 3", "Bilag 3"},
            {"Distribusjonsavtale", "Distribusjonsavtale"}
        };

        public static Dictionary<string, string> MeetingTypes = new Dictionary<string, string>
        {
            {"Partsoppfølging – fysisk møte", "Partsoppfølging – fysisk møte"},
            {"Partsoppfølging – web møte", "Partsoppfølging – web møte"},
        };

        public static Dictionary<string, string> Organizations = new Dictionary<string, string>
        {
            {"971040238", "Kartverket"},
            {"999601391", "Miljødirektoratet"},
            {"970188290", "Norges geologiske undersøkelse"}
        };
    }

}
