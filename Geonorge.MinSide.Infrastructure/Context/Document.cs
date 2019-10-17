using System;
using System.Collections.Generic;
using System.Text;

namespace Geonorge.MinSide.Infrastructure.Context
{
    class Document
    {
        public int ID { get; set; }
        public string OrganizationNumber { get; set; }
        public string GeneralTerms { get; set; }
        public string Appendix1 { get; set; }
        public string Appendix2 { get; set; }
        public string Appendix3 { get; set; }
    }
}
