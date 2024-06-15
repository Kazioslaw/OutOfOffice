using System.ComponentModel.DataAnnotations;

namespace OutOfOfficeHRApp.Models
{
    public class Project
    {
        [Key]
        public int ID { get; set; }
        [Required]
        public int ProjectTypeID { get; set; }
        public ProjectType ProjectType { get; set; }
        [Required]
        public DateOnly StartDate { get; set; }
        public DateOnly EndDate { get; set; }
        [Required]
        public int EmployeeID { get; set; }
        public Employee ProjectManager { get; set; }
        public string? Comment { get; set; }
        [Required]
        [Display(Name = "Status")]
        [DisplayFormat(DataFormatString = "{0:Active/Inactive}")]
        public bool IsActive { get; set; }
    }
}
