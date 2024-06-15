using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OutOfOfficeHRApp.Data;
using OutOfOfficeHRApp.Enums;
using OutOfOfficeHRApp.Models;

namespace OutOfOfficeHRApp.Controllers
{
    [Route("[controller]")]
    public class LeaveRequestController : Controller
    {
        private readonly OutOfOfficeContext _context;
        public LeaveRequestController(OutOfOfficeContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetLeaveRequests()
        {
            var leaveRequests = await _context.LeaveRequest.ToListAsync();
            return View("Index", leaveRequests);
        }

        [HttpGet("{id}")]

        public async Task<IActionResult> GetLeaveRequest(int id)
        {
            var leaveRequest = await _context.LeaveRequest.FindAsync(id);
            if (leaveRequest == null)
            {
                return NotFound();
            }
            return Ok(leaveRequest);
        }

        [HttpGet("Create")]
        public IActionResult AddLeaveRequest()
        {
            return View("Create");
        }

        [HttpPost("Create")]
        public async Task<IActionResult> AddLeaveRequest(LeaveRequestController leaveRequest)
        {
            _context.Add(leaveRequest);
            await _context.SaveChangesAsync();
            return Ok();
        }
        [HttpGet("Submit/{id}")]
        public IActionResult Submit(int id)
        {
            var leaveRequest = _context.LeaveRequest.Include(lr => lr.AbsenceReason).Include(lr => lr.Employee).Include(lr => lr.Status).FirstOrDefaultAsync(lr => lr.ID == id);
            return View("Submit", leaveRequest);
        }

        [HttpPost("Submit/{id}")]
        public async Task<IActionResult> SubmitRequest(int id)
        {
            var leaveRequest = await _context.LeaveRequest.FindAsync(id);
            if (leaveRequest == null)
            {
                return NotFound();
            }
            leaveRequest.Status = Status.Submitted;
            var approvalRequest = new ApprovalRequest
            {
                LeaveRequest = leaveRequest,
                EmployeeID = await _context.Employee.Where(e => e.Position.Name == "HR Manager").Select(e => e.ID).FirstOrDefaultAsync(),
            };
            _context.Update(leaveRequest);
            _context.Add(approvalRequest);
            await _context.SaveChangesAsync();
            return Ok();

        }

        [HttpGet("Cancel/{id}")]
        public IActionResult Cancel(int id)
        {
            var leaveRequest = _context.LeaveRequest.Include(lr => lr.AbsenceReason).Include(lr => lr.Employee).Include(lr => lr.Status).FirstOrDefaultAsync(lr => lr.ID == id);
            return View("Cancel", leaveRequest);
        }

        [HttpPost("Cancel/{id}")]
        public async Task<IActionResult> CancelRequest(int id)
        {
            var leaveRequest = await _context.LeaveRequest.FindAsync(id);
            if (leaveRequest == null)
            {
                return NotFound();
            }
            leaveRequest.Status = Status.Cancelled;
            _context.Update(leaveRequest);
            await _context.SaveChangesAsync();
            return Ok();
        }
    }
}
