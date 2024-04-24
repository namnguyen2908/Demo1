using Demo1.Areas.Data;

namespace Demo1.Models
{
    public class JobApplication
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Coverletter { get; set; }




        public int JobListingID { get; set; }
        public virtual JobListing? JobListing { get; set; }
        public string ApplicationUserId { get; set; }
        public virtual ApplicationUser? ApplicationUser { get; set; }


    }

}
