﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Geonorge.MinSide.Infrastructure.Context
{
    public class UserSettings
    {
        [Key]
        public string Username { get; set; }
        public string Email { get; set; }
        public string Organization { get; set; }
        public bool? TodoNotification { get; set; }
        public bool? TodoReminder { get; set; }
        public int? TodoReminderTime { get; set; }
    }
}
