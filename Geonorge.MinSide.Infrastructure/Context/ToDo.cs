using System;
using System.Collections.Generic;
using System.Text;

namespace Geonorge.MinSide.Infrastructure.Context
{
    class ToDo
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string ResponsibleOrganization { get; set; } //Can be several organizations?
        public DateTime Deadline { get; set; }
        public string Status { get; set; }  //Which statuses?
        public string Comment { get; set; }
        public DateTime Done { get; set; }
    }
}
