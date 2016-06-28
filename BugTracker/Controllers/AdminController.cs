using BugTracker.Helpers;
using BugTracker.Models;
using Microsoft.AspNet.Identity;
using System;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Collections.Generic;

namespace BugTracker.Controllers
{
    [RequireHttps]
    public class AdminController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Admin
        public ActionResult Index()
        {
            return View();
        }
        [Authorize(Roles = "Admin")]
        // GET: EditUser
        public ActionResult EditUser(string id)
        {
            var user = db.Users.Find(id);
            AdminUserViewModel AdminModel = new AdminUserViewModel();
            UserRolesHelper helper = new UserRolesHelper(db);
            var selected = helper.ListUserRoles(id);
            AdminModel.Roles = new MultiSelectList(db.Roles, "Name", "Name", selected);
            AdminModel.Id = user.Id;
            AdminModel.Name = user.FullName;

            return View(AdminModel);
        }
        [Authorize(Roles = "Admin")]
        //POST: EditUser/Assign Roles
        [HttpPost]
        public ActionResult EditUser(AdminUserViewModel model, string id)
        {
            UserRolesHelper helper = new UserRolesHelper(db);
            foreach(var role in model.SelectedRoles)
            {
                if (!helper.IsUserInRole(model.Id, role))
                {
                    helper.AddUserToRole(model.Id, role);
                }
            }
            
                foreach (var role in db.Roles.ToList())
                {
                if (!model.SelectedRoles.Contains(role.Name))
                {
                    helper.RemoveUserFromRole(model.Id, role.Name);
                }
                }

            db.SaveChanges();
            return RedirectToAction("UserIndex", "Admin", model);
        }
        //GET: UserIndex
        public ActionResult UserIndex()
        {
            UserRolesHelper helper = new UserRolesHelper(db);
            List<UserRolesViewModel> model = new List<UserRolesViewModel>();

            foreach (var user in db.Users.ToList())
            {
                UserRolesViewModel UserModel = new UserRolesViewModel();
                UserModel.Roles = helper.ListUserRoles(user.Id).ToList();
                UserModel.User = user;
                model.Add(UserModel);

            };
            return View(model);
        }
    }
}