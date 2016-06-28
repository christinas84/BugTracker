namespace BugTracker.Migrations
{
    using Microsoft.AspNet.Identity;
    using Microsoft.AspNet.Identity.EntityFramework;
    using Models;
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<BugTracker.Models.ApplicationDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = true;
        }

        protected override void Seed(BugTracker.Models.ApplicationDbContext context)
        {
            var roleManager = new RoleManager<IdentityRole>(
                new RoleStore<IdentityRole>(context));
            if (!context.Roles.Any(r => r.Name == "Admin"))
            {
                roleManager.Create(new IdentityRole { Name = "Admin" });
            }
            if (!context.Roles.Any(r => r.Name == "Project Manager"))
            {
                roleManager.Create(new IdentityRole { Name = "Project Manager" });
            }
            if (!context.Roles.Any(r => r.Name == "Developer"))
            {
                roleManager.Create(new IdentityRole { Name = "Developer" });
            }
            if (!context.Roles.Any(r => r.Name == "Submitter"))
            {
                roleManager.Create(new IdentityRole { Name = "Submitter" });
            }

            var userManager = new UserManager<ApplicationUser>(
                new UserStore<ApplicationUser>(context));
            if (!context.Users.Any(u => u.Email == "cesimmons84@gmail.com"))
            {
                userManager.Create(new ApplicationUser
                {
                    UserName = "cesimmons84@gmail.com",
                    Email = "cesimmons84@gmail.com",
                    FirstName = "Christina",
                    LastName = "Simmons",
                    DisplayName = "Christina"
                }, "Cjis749req!");

            }
            if (!context.Users.Any(u => u.Email == "demoadmin@gmail.com"))
            {
                userManager.Create(new ApplicationUser
                {
                    UserName = "demoadmin@gmail.com",
                    Email = "demoadmin@gmail.com",
                    FirstName = "Guest",
                    LastName = "Admin",
                    DisplayName = "Guest Admin"
                }, "demo1234");

            }
            if (!context.Users.Any(u => u.Email == "demoPM@gmail.com"))
            {
                userManager.Create(new ApplicationUser
                {
                    UserName = "demoPM@gmail.com",
                    Email = "demoPM@gmail.com",
                    FirstName = "Guest",
                    LastName = "PM",
                    DisplayName = "Guest PM"
                }, "demo1234");

            }
            if (!context.Users.Any(u => u.Email == "demodev@gmail.com"))
            {
                userManager.Create(new ApplicationUser
                {
                    UserName = "demodev@gmail.com",
                    Email = "demodev@gmail.com",
                    FirstName = "Guest",
                    LastName = "Developer",
                    DisplayName = "Guest Developer"
                }, "demo1234");

            }
            if (!context.Users.Any(u => u.Email == "demosub@gmail.com"))
            {
                userManager.Create(new ApplicationUser
                {
                    UserName = "demosub@gmail.com",
                    Email = "demosub@gmail.com",
                    FirstName = "Guest",
                    LastName = "Submitter",
                    DisplayName = "Guest Submitter"
                }, "demo1234");

            }
            var userId = userManager.FindByEmail("cesimmons84@gmail.com").Id;
            userManager.AddToRole(userId, "Admin");

        }





            //This method will be called after migrating to the latest version.

            //You can use the DbSet<T>.AddOrUpdate() helper extension method
            //to avoid creating duplicate seed data.E.g.

            //context.People.AddOrUpdate(
            //  p => p.FullName,
            //  new Person { FullName = "Andrew Peters" },
            //  new Person { FullName = "Brice Lambson" },
            //  new Person { FullName = "Rowan Miller" }
            //);

        }
    }

