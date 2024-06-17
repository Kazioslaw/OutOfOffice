using System.ComponentModel.DataAnnotations;

namespace OutOfOfficeHRApp.Models
{
    public class LeaveRequest
    {
        [Key]
        public int ID { get; set; }
        [Required]
        [Display(Name = "Employee")]
        public int EmployeeID { get; set; }
        public Employee Employee { get; set; }
        [Required]
        [Display(Name = "Absence Reason")]
        public int AbsenceReasonID { get; set; }
        public AbsenceReason AbsenceReason { get; set; }
        [Required]
        [Display(Name = "Start")]
        public DateOnly StartDate { get; set; }
        [Required]
        [Display(Name = "End")]
        public DateOnly EndDate { get; set; }
        public string? Comment { get; set; }
        [Required]
        public Status Status { get; set; } = Status.New;
    }
}
