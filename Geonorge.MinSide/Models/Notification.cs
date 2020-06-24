using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Geonorge.MinSide.Models
{
    public class Notification
    {
        public bool Send { get; set; }
        public string EmailCurrentUser { get; set; }
        public string UserNameCurrentUser { get; set; }
    }
}
