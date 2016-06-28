﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BugTracker.Models
{
    public class TicketsViewModel
    {
        public Tickets Ticket { get; set; }

        public MultiSelectList Users { get; set; }

        public string SelectedUsers { get; set; }
    }
}