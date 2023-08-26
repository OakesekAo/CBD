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
           
            //public only
            //var builds = _context.Build.Where(p => p.ReadyStatus == ReadyStatus.PublicReady).AsQueryable();

            var builds = _context.Build.AsQueryable();


            if (searchTerm != null)
            {
                searchTerm = searchTerm.ToLower();

                builds = builds.Where(
                    p => p.Name.ToLower().Contains(searchTerm) ||
                    p.CBDUser.Name.ToLower().Contains(searchTerm) ||
                    p.CBDUser.GlobalName.ToLower().Contains(searchTerm)
                    );
            }

            return builds.OrderByDescending(p => p.Created);

        }
    }
}
