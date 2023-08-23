using CBD.Data;
using CBD.Enums;
using CBD.Models;
using CBD.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace CBD.Helpers
{
    public class DataHelper
    {

        public static async Task ManageDataAsync(IServiceProvider svcProvider)
        {


            // get instance of the db application context
            var dbContextSvc = svcProvider.GetRequiredService<ApplicationDbContext>();
            var roleManager = svcProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = svcProvider.GetRequiredService<UserManager<CBDUser>>();

            //Migrate DB -- update-database
            await dbContextSvc.Database.MigrateAsync();


            // Call the instance methods
            if (!dbContextSvc.Roles.Any())
            {
                await SeedRolesAsync(roleManager);
            }

            //Check for users, if any, do nothing
            if (!dbContextSvc.Users.Any())
            {
            await SeedUsersAsync(userManager);                
            }

        }


        private static async Task SeedRolesAsync(RoleManager<IdentityRole> roleManager)
        {
            //Create a few roles
            foreach (var role in Enum.GetNames(typeof(CBDRole)))
            {
                await roleManager.CreateAsync(new IdentityRole(role));
            }
        }

        private static async Task SeedUsersAsync(UserManager<CBDUser> userManager)
        {



            var adminUser = new CBDUser()
            {
                Email = "o@o.com",
                UserName = "o@o.com",
                Name = "Admin",
                GlobalName = "@Admin",
                EmailConfirmed = true
            };

            //Create new user
            await userManager.CreateAsync(adminUser, "Abc&123!");

            //Add new user to admin role
            await userManager.AddToRoleAsync(adminUser, CBDRole.Administrator.ToString());

            var modUser = new CBDUser()
            {
                Email = "mod@o.com",
                UserName = "mod@o.com",
                Name = "Mod",
                GlobalName = "@Mod",
                EmailConfirmed = true
            };

            //Create new user
            await userManager.CreateAsync(modUser, "Abc&123!");

            //Add new user to admin role
            await userManager.AddToRoleAsync(modUser, CBDRole.Moderator.ToString());

        }


    }
}
