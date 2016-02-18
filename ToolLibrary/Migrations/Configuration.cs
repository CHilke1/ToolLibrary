namespace ToolLibrary.Migrations
{
    using Microsoft.AspNet.Identity;
    using Microsoft.AspNet.Identity.EntityFramework;
    using Microsoft.AspNet.Identity.Owin;
    using Models;
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;
    using System.Web;
    using System.Web.Providers.Entities;
    using System.Web.Security;
    internal sealed class Configuration : DbMigrationsConfiguration<ToolLibrary.DAL.ToolDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }


        //Create User=Admin@Admin.com with password=Admin@123456 in the Admin role

       protected override void Seed(ToolLibrary.DAL.ToolDbContext context)
        {
           /* const string defaultRole = "admin";
            const string defaultUser = "someUser";

            // This check for the role before attempting to add it.
            if (!context.Roles.Any(r => r.Name == defaultRole))
            {
                context.Roles.Add(new IdentityRole(defaultRole));
                context.SaveChanges();
            }

            // This check for the user before adding them.
            if (!context.Users.Any(u => u.UserName == defaultUser))
            {
                var store = new UserStore<ApplicationUser>(context);
                var manager = new UserManager<ApplicationUser>(store);
                var user = new ApplicationUser { UserName = defaultUser };
                manager.Create(user, "somePassword");

                manager.AddToRole(user.Id, defaultRole);
            }
            else
            {
                // Just for good measure, this adds the user to the role if they already
                // existed and just weren't in the role.
                var user = context.Users.Single(u => u.UserName.Equals(defaultUser, StringComparison.CurrentCultureIgnoreCase));
                var store = new UserStore<ApplicationUser>(context);
                var manager = new UserManager<ApplicationUser>(store);
                manager.AddToRole(user.Id, defaultRole);
            }*/
            //  This method will be called after migrating to the latest version.

            //  You can use the DbSet<T>.AddOrUpdate() helper extension method 
            //  to avoid creating duplicate seed data. E.g.
            //
            //    context.People.AddOrUpdate(
            //      p => p.FullName,
            //      new Person { FullName = "Andrew Peters" },
            //      new Person { FullName = "Brice Lambson" },
            //      new Person { FullName = "Rowan Miller" }
            //    );
            //
            // Access the application context and create result variables.

            /* var categories = new List<Category>
             {
                 new Category {
                     Name = "Automotive"
                 },
                 new Category {
                     Name = "Bike"
                 },
                 new Category {
                     Name = "Carpenty and Woodworking"
                 },
                 new Category {
                     Name = "Electrical and Soldering"
                 },
                 new Category {
                     Name = "Electronics"
                 },
                 new Category {
                     Name = "Home Maintenance"
                 },
                 new Category {
                     Name = "Metalworking"
                 },
                 new Category {
                     Name = "Plumbing"
                 },
                 new Category {
                     Name = "Remodeling"
                 },
                 new Category {
                     Name = "Sustainable Living"
                 },
                 new Category {
                     Name = "Workshop"
                 },
                 new Category {
                     Name = "Yard and Garden"
                 },
                 new Category {
                     Name = "Zombie Apocalypse"
                 }
             };
             categories.ForEach(c => context.Categories.Add(c)); 

                 var tools = new List<Tool>
                 {
                     new Tool {
                         Name = "Black & Decker No. 7144 2.9 Amp",
                         Description = "...",
                         Manufacturer = "Black and Decker",
                         ImageUrl = "http://ecx.images-amazon.com/images/I/51T%2BWt430bL._AA160_.jpg",
                         AdditionalDescription = "...",
                         Category = categories[2],
                         IsCheckedOut = false,
                         Type = ToolLibrary.Models.Type.Tools
                     },
                     new Tool {
                         Name = "Micrometer Adjustable Torque Wrench",
                         Description = "...",
                         Manufacturer = "Black and Decker",
                         ImageUrl = "http://ecx.images-amazon.com/images/I/51AkFkNeUxL._AA160_.jpg",
                         AdditionalDescription = "...",
                         Category = categories[2],
                         IsCheckedOut = false,
                         Type = ToolLibrary.Models.Type.Tools
                     },
                     new Tool {
                         Name = "Black & Decker Culti-Hoe",
                         Description = "...",
                         Manufacturer = "Black and Decker",
                         ImageUrl = "http://ecx.images-amazon.com/images/I/51LpqnDq8-L._AA160_.jpg",
                         AdditionalDescription = "...",
                         Category = categories[2],
                         IsCheckedOut = false,
                         Type = ToolLibrary.Models.Type.Tools
                     },
                     new Tool {
                         Name = "10\" crescent wrench",
                         Description = "...",
                         Manufacturer = "Crestory",
                         ImageUrl = "http://ecx.images-amazon.com/images/I/41JC54HEroL._AA160_.jpg",
                         AdditionalDescription = "...",
                         Category = categories[2],
                         IsCheckedOut = false,
                         Type = ToolLibrary.Models.Type.Tools
                     }
                 };
                 tools.ForEach(t => context.Tools.Add(t)); */

            //context.SaveChanges(); 
        } 
    }
}
