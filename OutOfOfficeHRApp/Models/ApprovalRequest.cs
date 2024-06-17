using System.ComponentModel.DataAnnotations;

namespace OutOfOfficeHRApp.Models
{
    public class ApprovalRequest
    {
        [Key]
        public int ID { get; set; }
        [Required]
        [Display(Name = "Approver")]
        public int EmployeeID { get; set; }
        public Employee Employee { get; set; }
        [Required]
        [Display(Name = "Leave Request")]
        public int LeaveRequestID { get; set; }
        public LeaveRequest LeaveRequest { get; set; }
        [Required]
        public Status Status { get; set; } = Status.New;
        public string? Comment { get; set; }
    }
}
