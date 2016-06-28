using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace BugTracker.Models
{
    public class TicketStatuses
    {
        public int Id { get; set; }
        [Display(Name = "Ticket Status")]
        public string Name { get; set; }
    }
}