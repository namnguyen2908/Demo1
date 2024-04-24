using Demo1.Models;
using Microsoft.AspNetCore.Identity;

namespace Demo1.Areas.Data
{
    public class ApplicationUser : IdentityUser
    {
        public string UserRole { get; set; }
        public virtual ICollection<JobApplication>? JobApplication { get; set; }

        public virtual ICollection<JobListing>? JobListing { get; set; }

    }
}
