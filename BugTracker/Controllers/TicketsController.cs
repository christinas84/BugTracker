using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using BugTracker.Models;
using Microsoft.AspNet.Identity;
using BugTracker.Helpers;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Configuration;
using SendGrid;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity.Owin;

namespace BugTracker.Controllers
{
    [RequireHttps]
    // [Authorize]
    public class TicketsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        private NotificationHelper notHelper = new NotificationHelper();

        private ApplicationUserManager _userManager;

        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

        // GET: Tickets
        public ActionResult Index()
        {
            UserRolesHelper helper = new UserRolesHelper(db);
            var user = User.Identity.GetUserId();
            var tickets = db.Tickets.Include(t => t.Project).Include(t => t.TicketPriority).Include(t => t.TicketStatus).Include(t => t.TicketType).Include(t => t.AssignedToUser).Include(t => t.OwnerUser);
            if (User.IsInRole("Admin"))
            {
                return View(tickets);
            }
            else if (helper.IsUserInRole(user, "Project Manager"))
            {
                
                //var tics = user.Project.SelectMany(p => p.Ticket);
                var tics = db.Tickets.Where(t => t.AssignedToUserId == user).ToList();


                return View(tics);
            }
            else if (helper.IsUserInRole(user, "Developer"))
            {

                //var tics = user.Project.SelectMany(p => p.Ticket);
                var tics = db.Tickets.Where(t => t.AssignedToUserId == user).ToList();


                return View(tics);
            }
            else
            {
                //var tics = user.Project.SelectMany(p => p.Ticket);
                var tics = db.Tickets.Where(t => t.OwnerUserId == user).ToList();

                return View(tics);
            }

        }

        // GET: Tickets/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Tickets tickets = db.Tickets.Find(id);
            if (tickets == null)
            {
                return HttpNotFound();
            }
            return View(tickets);
        }

        //GET: assign users to projects
        public ActionResult AssignUsers(int Id)
        {
            var project = db.Tickets.Find(Id);
            ProjectUserViewModel ViewModel = new ProjectUserViewModel();
            UserProjectsHelper helper = new UserProjectsHelper();
            var user = new ApplicationUser();
            var selected = helper.ListProjectUsers(Id).Select(a => a.Id).ToArray();
            ViewModel.Users = new MultiSelectList(db.Users, "Id", "DisplayName", selected);
            //ViewModel.Ticket = ticket;

            return View(ViewModel);
        }

        //POST: assign users to projects
        [HttpPost]
        [Authorize(Roles = "Admin, Project Manager")]

        public async Task<ActionResult> AssignUsers(TicketsViewModel model, int ticketId, Tickets tickets)
        {

            var ticket = db.Tickets.Find(ticketId);
            var oldTic = db.Tickets.AsNoTracking().FirstOrDefault(t => t.Id == ticket.Id);
            var userId = User.Identity.GetUserId();
            var userid = User.Identity.GetUserId();
            var changed = System.DateTime.Now;

            ticket.AssignedToUserId = model.SelectedUsers;
            ticket.Updated = DateTimeOffset.Now;
            db.SaveChanges(); foreach (var user in model.SelectedUsers)

            if (oldTic?.AssignedToUserId != ticket.AssignedToUserId)
                {
                    string oldAssignedToUserId = oldTic.AssignedToUserId;
                    string newAssignedToUser = db.Users.Find(ticket.AssignedToUserId).DisplayName;

                    TicketHistories th6 = new TicketHistories
                    {
                        TicketId = ticket.Id,
                        Property = "AssignedToUserId",
                        OldValue = oldTic?.AssignedToUser.DisplayName,
                        NewValue = ticket.AssignedToUser.DisplayName,
                        Changed = changed,
                        UserId = userid
                    };
                    db.TicketHistories.Add(th6);
                    db.SaveChanges();

                }
            ApplicationUser NotifyUser = db.Users.FirstOrDefault(u => u.Id.Equals(tickets.AssignedToUserId));
            //ApplicationUser NotifyUser = db.Users.Find(ticket.AssignedToUserId);


            if (NotifyUser != null /*|| !(await UserManager.IsEmailConfirmedAsync(user.Id))*/)
            {
                var callBackUrl = Url.Action("Details", "Tickets", new { id = tickets.Id }, protocol: Request.Url.Scheme);
                await UserManager.SendEmailAsync(NotifyUser.Id, "Ticket Update" + tickets.Id, "Good day, this is a notification that you have been assigned to this  <a href=\"" + callBackUrl + "\">ticket</a>.");
            }

            return RedirectToAction("Index", "Projects", model);
        }

        // GET: Tickets/Create
        public ActionResult Create(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Projects project = db.Project.Find(id);
            if (project == null)
            {
                return HttpNotFound();
            }
            ViewBag.ProjectId = new SelectList(db.Project, "Id", "Name");
            ViewBag.TicketPriorityId = new SelectList(db.TicketPriority, "Id", "Name");
            ViewBag.TicketStatusId = new SelectList(db.TicketStatus, "Id", "Name");
            ViewBag.TicketTypeId = new SelectList(db.TicketType, "Id", "Name");
            var ticket = new Tickets();
            ticket.ProjectId = (int)id;
            return View(ticket);
        }

        // POST: Tickets/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        //[Authorize]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Title,Description,ProjectId,TicketTypeId,TicketPriorityId,TicketStatusId")] Tickets tickets)
        {
            if (ModelState.IsValid)
            {
                tickets.OwnerUserId = User.Identity.GetUserId();
                tickets.Created = System.DateTimeOffset.Now;
                db.Tickets.Add(tickets);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.ProjectId = new SelectList(db.Project, "Id", "Name", tickets.ProjectId);
            ViewBag.TicketPriorityId = new SelectList(db.TicketPriority, "Id", "Name", tickets.TicketPriorityId);
            ViewBag.TicketStatusId = new SelectList(db.TicketStatus, "Id", "Name", tickets.TicketStatusId);
            ViewBag.TicketTypeId = new SelectList(db.TicketType, "Id", "Name", tickets.TicketTypeId);
            //return View(tickets);
            return RedirectToAction("Index");

        }
        [Authorize(Roles = "Admin, Developer, Project Manager, Submitter")]

        // GET: Tickets/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Tickets tickets = db.Tickets.Find(id);

            UserProjectsHelper helper = new UserProjectsHelper();
            var prjUsr = helper.ListProjectUsers(tickets.ProjectId);

            if (tickets == null)
            {
                return HttpNotFound();
            }
            tickets.Updated = System.DateTimeOffset.Now;
            ViewBag.ProjectId = new SelectList(db.Project, "Id", "Name", tickets.ProjectId);
            ViewBag.TicketPriorityId = new SelectList(db.TicketPriority, "Id", "Name", tickets.TicketPriorityId);
            ViewBag.TicketStatusId = new SelectList(db.TicketStatus, "Id", "Name", tickets.TicketStatusId);
            ViewBag.TicketTypeId = new SelectList(db.TicketType, "Id", "Name", tickets.TicketTypeId);
            ViewBag.AssignedToUserId = new SelectList(prjUsr, "Id", "FirstName", tickets.AssignedToUserId);


            return View(tickets);
        }






        // POST: Tickets/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "Id,Title,Description,Created,Updated,ProjectId,TicketTypeId,TicketAttachment,TicketPriorityId,TicketStatusId,OwnerUserId,AssignedToUserId")] Tickets tickets)
        {

            if (ModelState.IsValid)
            {
                ApplicationUser NotifyUser = db.Users.FirstOrDefault(u => u.Id.Equals(tickets.AssignedToUserId));

                var oldTic = db.Tickets.AsNoTracking().FirstOrDefault(t => t.Id == tickets.Id);
                tickets.Updated = DateTime.Now;
                db.Tickets.Attach(tickets);
                db.Entry(tickets).State = EntityState.Modified;
                db.SaveChanges();
                //return RedirectToAction("Index");

                var userid = User.Identity.GetUserId();
                var changed = System.DateTime.Now;


                if (oldTic?.Title != tickets.Title)
                {
                    TicketHistories th1 = new TicketHistories
                    {
                        TicketId = tickets.Id,
                        Property = "Title",
                        OldValue = oldTic?.Title,
                        NewValue = tickets.Title,
                        Changed = changed,
                        UserId = userid
                    };
                    db.TicketHistories.Add(th1);
                }

                if (oldTic?.Description != tickets.Description)
                {
                    TicketHistories th2 = new TicketHistories
                    {
                        TicketId = tickets.Id,
                        Property = "Description",
                        OldValue = oldTic?.Description,
                        NewValue = tickets.Description,
                        Changed = changed,
                        UserId = userid
                    };
                    db.TicketHistories.Add(th2);
                }

                //if (oldTic?.AssignedToUserId != tickets.AssignedToUserId)
                //{
                //    string oldAssignedToUserId = oldTic.AssignedToUserId;
                //    string newAssignedToUser = db.Users.Find(tickets.AssignedToUserId).DisplayName;

                //    TicketHistories th6 = new TicketHistories
                //    {
                //        TicketId = tickets.Id,
                //        Property = "AssignedToUserId",
                //        OldValue = oldTic?.AssignedToUser.DisplayName,
                //        NewValue = tickets.AssignedToUser.DisplayName,
                //        Changed = changed,
                //        UserId = userid
                //    };
                //    db.TicketHistories.Add(th6);
                //}

                if (oldTic?.TicketStatusId != tickets.TicketStatusId)
                {
                    string oldTicketStatus = oldTic.TicketStatus.Name;
                    string newTicketStatus = db.TicketStatus.Find(tickets.TicketStatusId).Name;

                    TicketHistories th3 = new TicketHistories
                    {
                        TicketId = tickets.Id,
                        Property = "Ticket Status",
                        OldValue = oldTicketStatus,
                        NewValue = newTicketStatus,
                        Changed = changed,
                        UserId = userid
                    };
                    db.TicketHistories.Add(th3);
                }

                if (oldTic?.TicketPriorityId != tickets.TicketPriorityId)
                {
                    string oldTicketPriority = oldTic.TicketPriority.Name;
                    string newTicketPriority = db.TicketPriority.Find(tickets.TicketPriorityId).Name;

                    TicketHistories th4 = new TicketHistories
                    {
                        TicketId = tickets.Id,
                        Property = "Ticket Priority",
                        OldValue = oldTicketPriority,
                        NewValue = newTicketPriority,
                        Changed = changed,
                        UserId = userid
                    };
                    db.TicketHistories.Add(th4);
                }

                if (oldTic?.TicketTypeId != tickets.TicketTypeId)
                {
                    string oldTicketType = oldTic.TicketType.Name;
                    string newTicketType = db.TicketType.Find(tickets.TicketTypeId).Name;

                    TicketHistories th5 = new TicketHistories
                    {
                        TicketId = tickets.Id,
                        Property = "Ticket Type",
                        OldValue = oldTicketType,
                        NewValue = newTicketType,
                        Changed = changed,
                        UserId = userid
                    };
                    db.TicketHistories.Add(th5);
                }
                
                db.SaveChanges();
                //await notHelper.Noto(tickets);

                if (NotifyUser != null /*|| !(await UserManager.IsEmailConfirmedAsync(user.Id))*/)
                {
                    var callBackUrl = Url.Action("Details", "Tickets", new { id = tickets.Id }, protocol: Request.Url.Scheme);
                    await UserManager.SendEmailAsync(NotifyUser.Id, "Ticket Update" + tickets.Id, "Good day, this is a notification of changes to this  <a href=\"" + callBackUrl + "\">ticket</a> you are assigned.");
                }                

                return RedirectToAction("Index");

            }
            ViewBag.ProjectId = new SelectList(db.Project, "Id", "Name", tickets.ProjectId);
            ViewBag.TicketPriorityId = new SelectList(db.TicketPriority, "Id", "Name", tickets.TicketPriorityId);
            ViewBag.TicketStatusId = new SelectList(db.TicketStatus, "Id", "Name", tickets.TicketStatusId);
            ViewBag.TicketTypeId = new SelectList(db.TicketType, "Id", "Name", tickets.TicketTypeId);
            return View(tickets);

        }


        // GET: Tickets/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Tickets tickets = db.Tickets.Find(id);
            if (tickets == null)
            {
                return HttpNotFound();
            }
            return View(tickets);
        }

        // POST: Tickets/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Tickets tickets = db.Tickets.Find(id);
            db.Tickets.Remove(tickets);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }


        public static class ImageUploadValidator
        {
            public static bool IsWebFriendlyImage(HttpPostedFileBase file)
            {
                if (file == null)
                    return false;
                if (file.ContentLength > 2 * 1024 * 1024 || file.ContentLength < 1024)
                    return false;
                try
                {
                    using (var img = Image.FromStream(file.InputStream))
                    {
                        return ImageFormat.Jpeg.Equals(img.RawFormat) ||
                               ImageFormat.Png.Equals(img.RawFormat) ||
                               ImageFormat.Gif.Equals(img.RawFormat);
                    }

                }
                catch
                {
                    return false;
                }
            }
           
    }





        // POST: TicketComments/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateTicketComment([Bind(Include = "Id,Comment,UserId,TicketId,Created")] TicketComments ticketComments)
        {
            if (ModelState.IsValid)
            {

                ticketComments.UserId = User.Identity.GetUserId();
                ticketComments.Created = DateTimeOffset.Now;

                db.TicketComments.Add(ticketComments);
                db.SaveChanges();

                return RedirectToAction("Index");
            }

            ViewBag.TicketId = new SelectList(db.Tickets, "Id", "Title", ticketComments.TicketId);
            ViewBag.UserId = new SelectList(db.Users, "Id", "FirstName", ticketComments.UserId);
            return View(ticketComments);
        }


        // POST: TicketAttachments/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> CreateTicketAttachments([Bind(Include = "Id,TicketId,UserId,FilePath,Description,Created,FileUrl")] TicketAttachments ticketAttachments, HttpPostedFileBase image)
        {
            if (ModelState.IsValid)
            {
                Tickets tickets = db.Tickets.FirstOrDefault();

                ApplicationUser NotifyUser = db.Users.FirstOrDefault(u => u.Id.Equals(tickets.AssignedToUserId));

                if (ImageUploadValidator.IsWebFriendlyImage(image))
                {
                    var fileName = Path.GetFileName(image.FileName);
                    image.SaveAs(Path.Combine(Server.MapPath("~/assets/Uploads/"), fileName));
                    ticketAttachments.FileUrl = "~/assets/Uploads/" + fileName;
                }
                ticketAttachments.UserId = User.Identity.GetUserId();
                ticketAttachments.Created = DateTimeOffset.Now;
                db.TicketAttachments.Add(ticketAttachments);
                db.SaveChanges();

                if (NotifyUser != null /*|| !(await UserManager.IsEmailConfirmedAsync(user.Id))*/)
                {
                    var callBackUrl = Url.Action("Details", "Tickets", new { id = ticketAttachments.Ticket.Id }, protocol: Request.Url.Scheme);
                    await UserManager.SendEmailAsync(NotifyUser.Id, "Ticket Update" + ticketAttachments.Ticket.Id, "Good day, this is a notification of changes to this  <a href=\"" + callBackUrl + "\">ticket</a> you area assigned.");
                }

                return RedirectToAction("Index");
            }

            ViewBag.TicketId = new SelectList(db.Tickets, "Id", "Title", ticketAttachments.TicketId);
            ViewBag.UserId = new SelectList(db.Users, "Id", "FirstName", ticketAttachments.UserId);
            return View(ticketAttachments);
        }

    }
}
