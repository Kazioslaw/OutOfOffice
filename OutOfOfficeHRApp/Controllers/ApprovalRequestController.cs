using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OutOfOfficeHRApp.Data;

namespace OutOfOfficeHRApp.Controllers
{

    [Route("[controller]")]

    public class ApprovalRequestController : Controller
    {
        private readonly OutOfOfficeContext _context;
        public ApprovalRequestController(OutOfOfficeContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetApprovalRequests(int page = 1)
        {
            int pageSize = 25;
            int totalItems = await _context.ApprovalRequest.CountAsync();

            ViewBag.TotalPages = (int)Math.Ceiling(totalItems / (double)pageSize);
            ViewBag.CurrentPage = page;
            ViewBag.PageSize = pageSize;

            var approvals = await _context.ApprovalRequest
                                          .Skip((page - 1) * pageSize)
                                          .Take(pageSize)
                                          .Include(ar => ar.Employee)
                                          .Include(ar => ar.LeaveRequest)
                                          .ThenInclude(ar => ar.Employee)
                                          .ToListAsync();

            return View("Index", approvals);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetApprovalDetails(int id)
        {
            var approval = await _context.ApprovalRequest.Include(ar => ar.Employee).Include(ar => ar.LeaveRequest).ThenInclude(lr => lr.AbsenceReason).FirstOrDefaultAsync(ar => ar.ID == id);
            if (approval == null)
            {
                NotFound();
                return RedirectToAction(nameof(GetApprovalRequests));
            }
            return View("Details", approval);
        }

        [HttpPost("{id}/Approve")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ApproveRequest(int id)
        {
            var approval = await _context.ApprovalRequest.Include(ar => ar.LeaveRequest).FirstOrDefaultAsync(ar => ar.ID == id);
            if (approval == null)
            {
                return NotFound();
            }
            var leaveRequest = await _context.LeaveRequest.FindAsync(approval.LeaveRequestID);
            var endDays = approval.LeaveRequest.EndDate.DayNumber;
            var employee = await _context.Employee.FirstOrDefaultAsync(e => e.ID == approval.EmployeeID);
            if (employee == null)
            {
                Console.WriteLine("EEEE nie ma pracownika?!");
                return NotFound();
            }
            if (leaveRequest == null) { return NotFound(); }
            leaveRequest.Status = Status.Approved;
            Console.WriteLine(approval.LeaveRequest.EndDate.DayNumber - approval.LeaveRequest.StartDate.DayNumber);
            employee.OutOfOfficeBalance -= approval.LeaveRequest.EndDate.DayNumber - approval.LeaveRequest.StartDate.DayNumber;
            approval.Status = Status.Approved;
            approval.LeaveRequest.Status = Status.Approved;
            _context.ApprovalRequest.Update(approval);
            _context.LeaveRequest.Update(leaveRequest);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        [HttpGet("{id}/Reject")]
        public async Task<IActionResult> Reject(int id)
        {
            var approval = await _context.ApprovalRequest.Include(ar => ar.LeaveRequest).ThenInclude(lr => lr.AbsenceReason).Include(ar => ar.Employee).FirstOrDefaultAsync(ar => ar.ID == id);
            return View("Reject", approval);
        }

        [HttpPost("{id}/Reject")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RejectRequest(int id)
        {
            var approval = await _context.ApprovalRequest.FindAsync(id);
            if (approval == null)
            {
                return NotFound();
            }
            approval.Status = Status.Rejected;
            var leaveRequest = await _context.LeaveRequest.FindAsync(approval.LeaveRequestID);
            if (leaveRequest == null)
            {
                return NotFound();
            }
            leaveRequest.Status = Status.Rejected;
            leaveRequest.Comment = approval.Comment;
            _context.ApprovalRequest.Update(approval);
            _context.LeaveRequest.Update(leaveRequest);
            await _context.SaveChangesAsync();
            return Ok(approval);
        }

    }
}
