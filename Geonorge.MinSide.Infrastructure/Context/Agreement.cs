using System;
using System.Collections.Generic;
using System.Text;

namespace Geonorge.MinSide.Infrastructure.Context
{
    public class Agreement
    {
        public int Id { get; set; }
        public string OrganizationNumber { get; set; }
        public Document AgreementDocument { get; set; }
        public Document Appendix1 { get; set; } // annual costs
        public Document Appendix2 { get; set; } // deliveries
        public Document Appendix3 { get; set; } // deviations
        public Document DistributionAgreement { get; set; }
        //Archived?
    }
}
