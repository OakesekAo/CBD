using CBD.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace CBD.Data
{
    public class ApplicationDbContext : IdentityDbContext<CBDUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public DbSet<CBD.Models.BuildOld>? BuildOld { get; set; }
        public DbSet<CBD.Models.Comment>? Comment { get; set; }
        public DbSet<CBD.Models.Server>? Server { get; set; }
        public DbSet<CBD.Models.Tag>? Tag { get; set; }
        public DbSet<CBD.Models.Build>? Build { get; set; }
    }
}