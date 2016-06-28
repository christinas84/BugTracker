using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using BugTracker.Models;
using BugTracker.Helpers;
using Microsoft.AspNet.Identity;

namespace BugTracker.Controllers
{
    [RequireHttps]
    [Authorize]
    public class ProjectsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Projects
        [Authorize(Roles = "Admin, Project Manager, Developer, Submitter")]

        public ActionResult Index(string id)
        {
            //var usr = db.Users.Find(id);
            var user = new ApplicationUser();
            user = db.Users.Find(User.Identity.GetUserId());
            UserRolesHelper helper = new UserRolesHelper(db);
            UserProjectsHelper helper2 = new UserProjectsHelper();
            if (User.IsInRole("Admin")|| (User.IsInRole("Submitter")))
            {
                return View(db.Project.ToList());
            }
            else if (User.IsInRole("Developer"))
            {
                return View(user.Project.ToList());
            }
            else if (User.IsInRole("Submitter"))
            {
                return View(db.Project.ToList());
            }
            else if (User.IsInRole("Project Manager"))
            {
                return View(user.Project.ToList());
            }
            else
            {
                return View();
            }
        }





        //GET: assign users to projects
        public ActionResult AssignUsers(int Id)
        {
            var project = db.Project.Find(Id);
            ProjectUserViewModel ViewModel = new ProjectUserViewModel();
            UserProjectsHelper helper = new UserProjectsHelper();
            var user = new ApplicationUser();
            var selected = helper.ListProjectUsers(Id).Select(a => a.Id).ToArray();
            ViewModel.Users = new MultiSelectList(db.Users, "Id", "DisplayName", selected);
            ViewModel.Project = project;

            return View(ViewModel);
        }

        //POST: assign users to projects
        [HttpPost]
        public ActionResult AssignUsers( ProjectUserViewModel model)
        {
            
            UserProjectsHelper helper = new UserProjectsHelper();
            foreach (var user in model.SelectedUsers)
            {
                if (!helper.IsUserOnProject(user, model.Project.Id))
                {
                    helper.AddUserToProject(user, model.Project.Id);
                }
            }
            foreach (var user in db.Users.ToList())
            {
                if (!model.SelectedUsers.Contains(user.Id))
                {
                    helper.RemoveUserFromProject(user.Id, model.Project.Id);
                }
            }

           
            db.SaveChanges();

            return RedirectToAction("Index", "Projects", model);
        }






        [Authorize(Roles = "Admin, Project Manager, Developer, Submitter")]
        // GET: Projects/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Projects projects = db.Project.Find(id);
            if (projects == null)
            {
                return HttpNotFound();
            }
            return View(projects);
        }

        // GET: Projects/Create
        [Authorize(Roles = "Admin")]
        public ActionResult Create()
        {
            return View();
        }

        // POST: Projects/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        //[Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Name,UserId,TicketId,Description")] Projects projects)
        {
            if (ModelState.IsValid)
            {
                projects.Created = System.DateTimeOffset.Now;
                db.Project.Add(projects);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(projects);
        }
        //[Authorize(Roles = "Admin")]
        // GET: Projects/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Projects projects = db.Project.Find(id);
            if (projects == null)
            {
                return HttpNotFound();
            }
            return View(projects);
        }
        
        [Authorize(Roles = "Admin")]
        // POST: Projects/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Name,Created,Description")] Projects projects)
        {
            if (ModelState.IsValid)
            {
                db.Project.Attach(projects);
                db.Entry(projects).State = EntityState.Modified;
                projects.Updated = System.DateTimeOffset.Now.LocalDateTime;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(projects);
        }
        [Authorize(Roles = "Admin")]
        // GET: Projects/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Projects projects = db.Project.Find(id);
            if (projects == null)
            {
                return HttpNotFound();
            }
            return View(projects);
        }
        [Authorize(Roles = "Admin")]
        // POST: Projects/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Projects projects = db.Project.Find(id);
            db.Project.Remove(projects);
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

        //// POST: TicketComments/Create
        //// To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        //// more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult CreateTicketComment([Bind(Include = "Id,Comment,UserId,TicketId,Created")] TicketComments ticketComments)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        ticketComments.UserId = User.Identity.GetUserId();
        //        ticketComments.Created = System.DateTimeOffset.Now;

        //        db.TicketComments.Add(ticketComments);
        //        db.SaveChanges();
               
        //        return RedirectToAction("Index");
        //    }

        //    ViewBag.TicketId = new SelectList(db.Tickets, "Id", "Title", ticketComments.TicketId);
        //    ViewBag.UserId = new SelectList(db.Users, "Id", "FirstName", ticketComments.UserId);
        //    return View(ticketComments);
        //}

        // GET: TicketComments/Edit/5
        public ActionResult EditTicketComment(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TicketComments ticketComments = db.TicketComments.Find(id);
            if (ticketComments == null)
            {
                return HttpNotFound();
            }
            ViewBag.TicketId = new SelectList(db.Tickets, "Id", "Title", ticketComments.TicketId);
            ViewBag.UserId = new SelectList(db.Users, "Id", "FirstName", ticketComments.UserId);
            return View(ticketComments);
        }

        // POST: TicketComments/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditComment([Bind(Include = "Id,Comment,UserId,TicketId,Created")] TicketComments ticketComments)
        {
            if (ModelState.IsValid)
            {
                db.Entry(ticketComments).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.TicketId = new SelectList(db.Tickets, "Id", "Title", ticketComments.TicketId);
            ViewBag.UserId = new SelectList(db.Users, "Id", "FirstName", ticketComments.UserId);
            return View(ticketComments);
        }

        // GET: TicketComments/Delete/5
        public ActionResult DeleteTicketComment(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TicketComments ticketComments = db.TicketComments.Find(id);
            if (ticketComments == null)
            {
                return HttpNotFound();
            }
            return View(ticketComments);
        }


    }
}
