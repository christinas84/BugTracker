using BugTracker.Models;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;

namespace BugTracker.Controllers
{
    [RequireHttps]
    public class HomeController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();


        //landing page
        public ActionResult Index(DemoLogin model)
        {
            //var user = new ApplicationUser();
            model.AdminEmail = "demoadmin@gmail.com";
            model.AdminPassword = "demo1234";

            model.PMEmail = "demoPM@gmail.com";
            model.PMPassword = "demo1234";

            model.DevEmail = "demodev@gmail.com";
            model.DevPassword = "demo1234";

            model.SubEmail = "demosub@gmail.com";
            model.SubPassword = "demo1234";           

            return View(model);
        }      

        //dashboard for logged in users
        [Authorize]
        public ActionResult Dashboard()
        {
            
            //db.Project.ToList();
            return View(db.Project.ToList());
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}