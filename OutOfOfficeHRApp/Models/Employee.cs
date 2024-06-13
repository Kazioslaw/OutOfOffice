using System.ComponentModel.DataAnnotations;

namespace OutOfOfficeHRApp.Models
{
    public class Employee
    {
        [Key]
        public int ID { get; set; }
        [Required]
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
        public int OutOfOfficeBalace { get; set; }
        public string Photo { get; set; }
    }
}
