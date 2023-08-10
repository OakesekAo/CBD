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
        private readonly ApplicationDbContext _dbContext;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<CBDUser> _userManager;

        public DataHelper(ApplicationDbContext dbContext, RoleManager<IdentityRole> roleManager, UserManager<CBDUser> userManager)
        {
            _dbContext = dbContext;
            _roleManager = roleManager;
            _userManager = userManager;
        }

        public static async Task ManageDataAsync(IServiceProvider svcProvider)
        {


            // get instance of the db application context
            var dbContextSvc = svcProvider.GetRequiredService<ApplicationDbContext>();

            //Migrate DB -- update-database
            await dbContextSvc.Database.MigrateAsync();

            // Create an instance of DataHelper
            var dataHelper = new DataHelper(_dbContext, _roleManager, _userManager);

            // Call the instance methods
            await dataHelper.SeedRolesAsync();
            await dataHelper.SeedUsersAsync();

        }


        private async Task SeedRolesAsync()
        {
            //Check for roles, if any, do nothing
            if (_dbContext.Roles.Any())
            {
                return;
            }
            //Create a few roles
            foreach (var role in Enum.GetNames(typeof(CBDRole)))
            {
                await _roleManager.CreateAsync(new IdentityRole(role));
            }
        }

        private async Task SeedUsersAsync()
        {
            //Check for users, if any, do nothing
            if (_dbContext.Users.Any())
            {
                return;
            }


            var adminUser = new CBDUser()
            {
                Email = "o@o.com",
                UserName = "o@o.com",
                FirstName = "Andrew",
                LastName = "O",
                GlobalName = "@Andrew",
                EmailConfirmed = true
            };

            //Create new user
            _userManager.CreateAsync(adminUser, "Abc&123!");

            //Add new user to admin role
            _userManager.AddToRoleAsync(adminUser, CBDRole.Administrator.ToString());

            var modUser = new CBDUser()
            {
                Email = "mod@o.com",
                UserName = "mod@o.com",
                FirstName = "AndrewMod",
                LastName = "O",
                GlobalName = "@AndrewMod",
                EmailConfirmed = true
            };

            //Create new user
            _userManager.CreateAsync(modUser, "Abc&123!");

            //Add new user to admin role
            _userManager.AddToRoleAsync(modUser, CBDRole.Moderator.ToString());

        }


    }
}
