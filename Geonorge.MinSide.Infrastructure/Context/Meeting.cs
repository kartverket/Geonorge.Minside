using Geonorge.MinSide.Infrastructure.Properties;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Geonorge.MinSide.Infrastructure.Context
{
    public class Meeting
    {
        public int Id { get; set; }
        [Display(Name = "OrganizationNumber", ResourceType = typeof(Resources))]
        public string OrganizationNumber { get; set; }
        [Display(Name = "Date", ResourceType = typeof(Resources))]
        public DateTime Date { get; set; }
        public string Type { get; set; }
        [Display(Name = "Description", ResourceType = typeof(Resources))]
        public string Description { get; set; }
        public List<Document> Documents { get; set; }
        public List<ToDo> ToDo { get; set; }
        [Display(Name = "Conclusion", ResourceType = typeof(Resources))]
        public string Conclusion { get; set; }
    }
}
