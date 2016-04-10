using EhrgoHealth.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace EhrgoHealth.Web.App_Start
{
    public static class DatabaseBootstrap
    {
        public static void Bootstrap(ApplicationDbContext db)
        {
            //roles the database should have ahead of time
            var defaultRoles = new List<string> { "Staff", "Patient" };
            defaultRoles
                //tolist here to force EF to fire off a DB query
                .Except(db.Roles.Select(a => a.Name).ToList())
                .ForEach(a => db.Roles.Add(new Microsoft.AspNet.Identity.EntityFramework.IdentityRole(a)));
            db.SaveChanges();
        }
    }
}