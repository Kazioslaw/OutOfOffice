using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OutOfOfficeHRApp.Models
{
    public class Employee
    {
        [Key]
        public int ID { get; set; }
        [Required]
        [Display(Name = "Full Name")]
        public string FullName { get; set; }
        [Required]
        public int SubdivisionID { get; set; }
        public Subdivision Subdivision { get; set; }
        [Required]
        public int PositionID { get; set; }
        public Position Position { get; set; }
        [Required]
        [Display(Name = "Status")]
        [DisplayFormat(DataFormatString = "{0:Active/Inactive}")]
        public bool IsActive { get; set; }
        [Required]
        public int? PeoplePartnerID { get; set; }
        [Display(Name = "HR Manager")]
        public Employee? PeoplePartner { get; set; }
        [Required]
        [Display(Name = "Days off")]
        public int OutOfOfficeBalance { get; set; }
        [NotMapped]
        public IFormFile? Photo { get; set; }
        [Display(Name = "Photo")]
        public string? PhotoPath { get; set; }
    }
}
