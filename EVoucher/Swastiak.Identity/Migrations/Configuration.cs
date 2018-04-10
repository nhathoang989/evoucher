namespace Identity.Migrations
{
    using Identity.Models;
    using Microsoft.AspNet.Identity;
    using Microsoft.AspNet.Identity.EntityFramework;
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<ApplicationDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }

        protected override void Seed(ApplicationDbContext context)
        {
            var manager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(new ApplicationDbContext()));
            var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(new ApplicationDbContext()));
            //  This method will be called after migrating to the latest version.
            roleManager.Create(new Microsoft.AspNet.Identity.EntityFramework.IdentityRole
            {
                Id = Guid.NewGuid().ToString("N"),
                Name = "Admin",
            });

            roleManager.Create(new Microsoft.AspNet.Identity.EntityFramework.IdentityRole
            {
                Id = Guid.NewGuid().ToString("N"),
                Name = "Manager",
            });

            var user = new ApplicationUser()
            {
                UserName = "Admin",
                Email = "admin@swastika.com",
            };
            
            manager.Create(user, "123123");

            manager.AddToRole(user.Id, "Admin");
            manager.AddToRole(user.Id, "Manager");
        }
    }
}
