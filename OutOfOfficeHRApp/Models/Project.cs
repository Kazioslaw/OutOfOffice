using System.ComponentModel.DataAnnotations;

namespace OutOfOfficeHRApp.Models
{
    public class Project
    {
        [Key]
        public int ID { get; set; }
        [Required]
        [Display(Name = "Project Type")]
        public int ProjectTypeID { get; set; }
        public ProjectType ProjectType { get; set; }
        [Required]
        [Display(Name = "Start")]
        public DateOnly StartDate { get; set; }
        [Display(Name = "End")]
        public DateOnly EndDate { get; set; }
        [Required]
        [Display(Name = "Project Manager")]
        public int EmployeeID { get; set; }
        public Employee ProjectManager { get; set; }
        public string? Comment { get; set; }
        [Required]
        [Display(Name = "Status")]
        [DisplayFormat(DataFormatString = "{0:Active/Inactive}")]
        public bool IsActive { get; set; }
    }
}
