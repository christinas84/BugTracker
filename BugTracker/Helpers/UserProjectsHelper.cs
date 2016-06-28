using BugTracker.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BugTracker.Helpers
{
    public class UserProjectsHelper
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        private Projects projects = new Projects();
        
        public void AddUserToProject(string userId, int projectId)
        {
            var proj = db.Project.Find(projectId);
            var user = db.Users.Find(userId);
            proj.Users.Add(user);
            db.SaveChanges();

        }
        public bool IsUserOnProject(string userId, int projectId)
        {
            var user = db.Users.Find(userId);
            var proj = db.Project.Find(projectId);
            if (proj.Users.Contains(user))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public void RemoveUserFromProject(string userId, int projectId)
        {
            var user = db.Users.Find(userId);
            var proj = db.Project.Find(projectId);
            if (proj.Users.Contains(user))
            {
                proj.Users.Remove(user);
                db.SaveChanges();
            }
        }
        //list of all projects for a given user
        public List<Projects> ListUserProjects(string userId)
        {
            var user = db.Users.Find(userId);
            projects.Users.Contains(user);
            return (user.Project.ToList());
        }

        public List<ApplicationUser> ListProjectUsers(int projectId)
        {
            var project = db.Project.Find(projectId);
            if (projects.Users == null)
            {
                return (new List<ApplicationUser>());
            }
            return (project.Users.ToList());
        }
        public List<ApplicationUser> ListOfUsersNotOnProjects()
        {

            var users = new List<ApplicationUser>();
            foreach (var p in projects.Users.Where(u => u.Id == null).ToList())
            {
                 users.Add(p);
            }
            return (users);
        }

        public List<ApplicationUser> ListOfUsersNotOnProject(int projectId)
        {
            //var project = db.Project.Find(projectId);
            //if(project.Users == null)
            //{
            //    project.Users.ToList();
            //}
            //return (project.Users.ToList());
            return db.Users.Where(u => u.Project.All(p => p.Id != projectId)).ToList();

        }
    }
}