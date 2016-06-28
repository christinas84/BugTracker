using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace BugTracker.Models
{
    public class Tickets
    {
        public Tickets()
        {
            this.TicketComments = new HashSet<TicketComments>();       
            this.TicketAttachments = new HashSet<TicketAttachments>();
            this.TicketHistories = new HashSet<TicketHistories>();
            this.TicketNotifications = new HashSet<TicketNotifications>();
        }       
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTimeOffset Created { get; set; }
        public DateTimeOffset? Updated { get; set; }
        public int ProjectId { get; set; }
        [Display(Name = "Ticket Type")]
        public int TicketTypeId { get; set; }
        [Display(Name = "Ticket Priority")]
        public int TicketPriorityId { get; set; }
        [Display(Name = "Ticket Status")]
        public int TicketStatusId { get; set; }
        [Display(Name = "Owner")]
        public string OwnerUserId { get; set; }
        [Display(Name = "Assigned")]
        public string AssignedToUserId { get; set; }

        public virtual ICollection<TicketComments> TicketComments { get; set; }
        public virtual ICollection<TicketAttachments> TicketAttachments { get; set; }
        public virtual ICollection<TicketHistories> TicketHistories { get; set; }
        public virtual ICollection<TicketNotifications> TicketNotifications { get; set; }
        public virtual Projects Project { get; set; }
        public virtual TicketTypes TicketType { get; set; }
        public virtual TicketPriorities TicketPriority { get; set; }
        public virtual TicketStatuses TicketStatus { get; set; }
        public virtual ApplicationUser OwnerUser { get; set; }
        public virtual ApplicationUser AssignedToUser { get; set; }
    }
}