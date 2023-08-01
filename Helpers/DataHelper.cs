using CBD.Data;
using Microsoft.EntityFrameworkCore;

namespace CBD.Helpers
{
    public class DataHelper
    {
        public static async Task ManageDataAsync(IServiceProvider svcProvider)
        {
            // get instance of the db application context
            var dbContextSvc = svcProvider.GetRequiredService<ApplicationDbContext>();

            //Migrate DB -- update-database
            await dbContextSvc.Database.MigrateAsync();
        }
    }
}
