using BugTracker.Models;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace BugTracker.Helpers
{
    public class NotificationHelper
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        public async Task<bool> Noto(Tickets tickets)
        {
            //ApplicationUser assignee = db.Users.FirstOrDefault(u => u.Id.Equals(tickets.AssignedToUserId));
            
                TicketNotifications tn5 = new TicketNotifications
                {
                    TicketId = tickets.Id,
                    UserId = tickets.AssignedToUserId
                };
                db.TicketNotifications.Add(tn5);

                db.SaveChanges();

                var assignee = db.Users.Find(tickets.AssignedToUserId);
                var es = new EmailService();
                var im = new IdentityMessage();
                im.Destination = assignee.Email;
                im.Body = "Notification of changes";
                await es.SendAsync(im);
                return true;


        }

    }
}