using OutOfOfficeHRApp.Enums;
using System.ComponentModel.DataAnnotations;

namespace OutOfOfficeHRApp.Models
{
    public class LeaveRequest
    {
        [Key]
        public int ID { get; set; }
        [Required]
        public int EmployeeID { get; set; }
        public Employee Employee { get; set; }
        [Required]
        public int ReasonID { get; set; }
        public AbsenceReason AbsenceReason { get; set; }
        [Required]
        public DateOnly StartDate { get; set; }
        [Required]
        public DateOnly EndDate { get; set; }
        public string Comment { get; set; }
        [Required]
        public Status Status { get; set; } = Status.New;
    }
}
