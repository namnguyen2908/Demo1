
using Demo1.Areas.Data;
using System.ComponentModel.DataAnnotations.Schema;

namespace Demo1.Models
{
    public class JobListing
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string RequiredQualifications { get; set; }
        public DateTime ApplicationDeadline { get; set; }



        public string ApplicationUserId { get; set; }
        public virtual ApplicationUser? ApplicationUser { get; set; }
        public virtual ICollection<JobApplication>? JobApplication { get; set; }
    }

}
