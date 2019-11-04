using System;
using System.Collections.Generic;
using System.Text;

namespace Geonorge.MinSide.Infrastructure.Context
{
    public class ToDo
    {
        public int Id { get; set; }
        public int Number { get; set; }
        public string Description { get; set; }
        public string ResponsibleOrganization { get; set; } 
        public DateTime Deadline { get; set; }
        public string Status { get; set; }  
        public string Comment { get; set; }
        public DateTime? Done { get; set; }
    }
}
