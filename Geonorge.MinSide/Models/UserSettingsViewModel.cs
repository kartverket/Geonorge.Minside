using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Geonorge.MinSide.Models
{
    public class UserSettingsViewModel
    {
        public string Email { get; set; }
        public bool TodoNotification { get; set; }
        public bool TodoReminder { get; set; }
        public int TodoReminderTime { get; set; }
    }
}
