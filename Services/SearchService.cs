using CBD.Data;
using CBD.Enums;
using CBD.Models;
using System.Configuration;

namespace CBD.Services
{
    public class SearchService
    {

        private readonly ApplicationDbContext _context;

        public SearchService(ApplicationDbContext context)
        {
            _context = context;
        }

        public IQueryable<Build> Search(string searchTerm)
        {

            // Ensure the search term is lowercase for case-insensitive search
            if (!string.IsNullOrEmpty(searchTerm))
            {
                searchTerm = searchTerm.ToLower();
            }


            //public only
            //var builds = _context.Build.Where(p => p.ReadyStatus == ReadyStatus.PublicReady).AsQueryable();

            var builds = _context.Build.AsQueryable();


            if (searchTerm != null)
            {
                builds = builds.Where(build =>
                                build.Name.ToLower().Contains(searchTerm) || // Search in build names
                                build.CBDUser.Name.ToLower().Contains(searchTerm) || // Search in user names
                                build.CBDUser.GlobalName.ToLower().Contains(searchTerm) || // Search in global user names
                                build.Class.ToLower().Contains(searchTerm) || // Search in class names
                                build.PowerSets.Any(ps => // Search in power sets
                                    ps.Name.ToLower().Contains(searchTerm) ||
                                    ps.NameDisplay.ToLower().Contains(searchTerm)
    )
);


                //builds = builds.Where(
                //    p => p.Name.ToLower().Contains(searchTerm) ||
                //    p.CBDUser.Name.ToLower().Contains(searchTerm) ||
                //    p.CBDUser.GlobalName.ToLower().Contains(searchTerm)
                //    );
            }

            return builds.OrderByDescending(p => p.Created);

        }
    }
}
