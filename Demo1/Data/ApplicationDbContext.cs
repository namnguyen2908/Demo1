using Demo1.Areas.Data;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Demo1.Models;

namespace Demo1.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public DbSet<Demo1.Models.JobListing> JobListing { get; set; } = default!;
        public DbSet<Demo1.Models.JobApplication> JobApplication { get; set; } = default!;
    }
}
