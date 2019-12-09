using Geonorge.MinSide.Infrastructure.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Geonorge.MinSide.Models
{
    public class MeetingViewModel
    {
        public Meeting Last { get; set; }
        public List<Meeting> Archive { get; set; }
    }
}
