using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Geonorge.MinSide.Infrastructure.Context
{
    public class InfoText
    {
        public int Id { get; set; }
        [Required]
        public string OrganizationNumber { get; set; }
        public string Text { get; set; }
    }
}
