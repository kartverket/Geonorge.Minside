using Geonorge.MinSide.Infrastructure.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Geonorge.MinSide.Models
{
    public class DocumentViewModel
    {
        public List<Document> Drafts { get; set; }
        public List<Document> Valid { get; set; }
        public List<Document> Superseded { get; set; }
    }
}
