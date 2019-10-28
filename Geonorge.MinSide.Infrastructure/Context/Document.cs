using System;
using System.Collections.Generic;
using System.Text;

namespace Geonorge.MinSide.Infrastructure.Context
{
    public class Document
    {
        public int Id { get; set; }
        public string OrganizationNumber { get; set; }
        public string Type { get; set; }
        public string Name { get; set; }
        public string FileName { get; set; }
        public DateTime Date { get; set; }
        public string Status { get; set; } 
    }
}
