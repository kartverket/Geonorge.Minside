using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Geonorge.MinSide.Models
{
    public class CodeList
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

        static CodeList() { UpdateOrganizations(); }

        public static Dictionary<string, string> Organizations = new Dictionary<string, string>();

        public static void UpdateOrganizations()
        {
            if(Organizations.Count == 0) { 

            Dictionary<string, string> OrganizationsList = new Dictionary<string, string>();

            string url = "https://register.geonorge.no/api/organisasjoner.json"; // Todo filter more norway digital member?
            System.Net.WebClient c = new System.Net.WebClient();
            c.Encoding = System.Text.Encoding.UTF8;
            var data = c.DownloadString(url);
            var response = Newtonsoft.Json.Linq.JObject.Parse(data);

            var codeList = response["containeditems"];

            foreach (var code in codeList)
            {
                JToken uuidToken = code["number"];

                JToken nameToken = code["label"];

                if(uuidToken != null && nameToken != null)
                { 
                    string uuid = uuidToken?.ToString();
                    string name = nameToken?.ToString();

                    if (!OrganizationsList.ContainsKey(uuid))
                        OrganizationsList.Add(uuid, name);
                }

            }

            OrganizationsList = OrganizationsList.OrderBy(o => o.Value).ToDictionary(o => o.Key, o => o.Value);

            Organizations = OrganizationsList;

            }
        }

        //public static Dictionary<string, string> Organizations = new Dictionary<string, string>
        //{
        //    {"971040238", "Kartverket"},
        //    {"999601391", "Miljødirektoratet"},
        //    {"970188290", "Norges geologiske undersøkelse"}
        //};
    }

}
