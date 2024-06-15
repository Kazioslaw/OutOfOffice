using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OutOfOfficeHRApp.Data;
using OutOfOfficeHRApp.Enums;

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
        public async Task<IActionResult> GetApprovalRequests()
        {
            var approvals = await _context.ApprovalRequest.ToListAsync();
            return View("Index", approvals);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetApprovalRequest(int id)
        {
            var approval = await _context.ApprovalRequest.FindAsync(id);
            if (approval == null)
            {
                return NotFound();
            }
            return Ok(approval);
        }

        [HttpGet("{id}/Approve")]
        public async Task<IActionResult> Approve(int id)
        {
            var request = await _context.ApprovalRequest.Include(ar => ar.LeaveRequest).FirstOrDefaultAsync(e => e.ID == id);
            return View("Approve", request);
        }

        [HttpPut("{id}/Approve")]
        public async Task<IActionResult> ApproveRequest(int id)
        {
            var approval = await _context.ApprovalRequest.FindAsync(id);
            if (approval == null)
            {
                return NotFound();
            }
            approval.Status = Status.Approved;
            _context.Update(approval);
            await _context.SaveChangesAsync();
            return Ok(approval);
        }


        [HttpGet("{id}/Reject")]
        public async Task<IActionResult> Reject(int id)
        {
            var request = await _context.ApprovalRequest.Include(ar => ar.LeaveRequest).FirstOrDefaultAsync(e => e.ID == id);
            return View("Reject", request);
        }

        [HttpPut("{id}/Reject")]
        public async Task<IActionResult> RejectRequest(int id)
        {
            var approval = await _context.ApprovalRequest.FindAsync(id);
            if (approval == null)
            {
                return NotFound();
            }
            approval.Status = Status.Rejected;
            _context.Update(approval);
            await _context.SaveChangesAsync();
            return Ok(approval);
        }

    }
}
