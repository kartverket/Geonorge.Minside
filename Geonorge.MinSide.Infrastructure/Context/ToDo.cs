using Geonorge.MinSide.Infrastructure.Properties;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Geonorge.MinSide.Infrastructure.Context
{
    public class ToDo
    {
        public int Id { get; set; }
        [Display(Name = "Number", ResourceType = typeof(Resources))]
        public int Number { get; set; }
        [Display(Name = "Subject", ResourceType = typeof(Resources))]
        public string Subject { get; set; }
        [Display(Name = "Description", ResourceType = typeof(Resources))]
        public string Description { get; set; }
        [Display(Name = "ResponsibleOrganization", ResourceType = typeof(Resources))]
        public string ResponsibleOrganization { get; set; }
        [Display(Name = "Deadline", ResourceType = typeof(Resources))]
        public DateTime Deadline { get; set; }
        public string Status { get; set; }
        [Display(Name = "Comment", ResourceType = typeof(Resources))]
        public string Comment { get; set; }
        [Display(Name = "Done", ResourceType = typeof(Resources))]
        public DateTime? Done { get; set; }
        [Display(Name = "OrganizationNumber", ResourceType = typeof(Resources))]
        public string OrganizationNumber { get; set; }
        public int? MeetingId { get; set; }
    }
}
