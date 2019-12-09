using Geonorge.MinSide.Infrastructure.Properties;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Geonorge.MinSide.Infrastructure.Context
{
    public class Document
    {
        public int Id { get; set; }
        [Display(Name = "OrganizationNumber", ResourceType = typeof(Resources))]
        public string OrganizationNumber { get; set; }
        public string Type { get; set; }
        [Display(Name = "Name", ResourceType = typeof(Resources))]
        public string Name { get; set; }
        [Display(Name = "FileName", ResourceType = typeof(Resources))]
        public string FileName { get; set; }
        [Display(Name = "Date", ResourceType = typeof(Resources))]
        public DateTime Date { get; set; }
        public string Status { get; set; } 
    }
}
