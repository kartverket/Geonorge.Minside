using System;
using System.Collections.Generic;
using System.Text;

namespace Geonorge.MinSide.Infrastructure.Context
{
    public class Meeting
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public List<Document> Documents { get; set; }
        public List<ToDo> ToDo { get; set; }
        public string Conclusion { get; set; }
    }
}
