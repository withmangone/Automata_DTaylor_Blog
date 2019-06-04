namespace Automata_DTaylor_Blog.Migrations
{
    using Automata_DTaylor_Blog.Models;
    using Microsoft.AspNet.Identity;
    using Microsoft.AspNet.Identity.EntityFramework;
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<Automata_DTaylor_Blog.Models.ApplicationDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = true;
        }

        protected override void Seed(Automata_DTaylor_Blog.Models.ApplicationDbContext context)
        {
            //  This method will be called after migrating to the latest version.

            //  You can use the DbSet<T>.AddOrUpdate() helper extension method 
            //  to avoid creating duplicate seed data.

            //I want to write some code that'll allow me to introduce a few roles
            var roleManager = new RoleManager<IdentityRole>(
                new RoleStore<IdentityRole>(context));

            if (!context.Roles.Any(r => r.Name == "Admin"))
            {
                roleManager.Create(new IdentityRole { Name = "Admin" });
            }

            if (!context.Roles.Any(r => r.Name == "Moderator"))
            {
                roleManager.Create(new IdentityRole { Name = "Moderator" });
            }

            //I want to write some code that'll allow me to introduce a few users
            var userManager = new UserManager<ApplicationUser>(
                new UserStore<ApplicationUser>(context));

            if (!context.Users.Any(u => u.Email == "wake.drew@gmail.com"))
            {
                #    region Helper Code- leave for ref
                //var user = new ApplicationUser
                //{
                //    Email = "wake.drew@gmail.com",
                //    UserName = "wake.drew@gmail.com",
                //    FirstName = "Drew",
                //    LastName = "Taylor",
                //    DisplayName = "withmangone"
                //};            
                //userManager.Create(user, "Govegan1!");
#endregion
                userManager.Create(new ApplicationUser
                {
                    Email = "wake.drew@gmail.com",
                    UserName = "wake.drew@gmail.com",
                    FirstName = "Drew",
                    LastName = "Taylor",
                    DisplayName = "withmangone"
                }, "Govegan1!");
            }

            var userId = userManager.FindByEmail("wake.drew@gmail.com").Id;
            userManager.AddToRole(userId, "Admin");

            //Add code to add another user of role moderater
            if (!context.Users.Any(u => u.Email == "wake.drew@mailinator.com"))
            {
                #region Helper Code- leave for ref
                //var user = new ApplicationUser
                //{
                //    Email = "wake.drew@gmail.com",
                //    UserName = "wake.drew@gmail.com",
                //    FirstName = "Drew",
                //    LastName = "Taylor",
                //    DisplayName = "withmangone"
                //};            
                //userManager.Create(user, "Govegan1!");
                #endregion
                userManager.Create(new ApplicationUser
                {
                    Email = "wake.drew@mailinator.com",
                    UserName = "wake.drew@mailinator.com",
                    FirstName = "Drog",
                    LastName = "Taylog",
                    DisplayName = "withmangog"
                }, "Govegan1!");
            }

            userId = userManager.FindByEmail("wake.drew@mailinator.com").Id;
            userManager.AddToRole(userId, "Moderator");
        }
    }
}
