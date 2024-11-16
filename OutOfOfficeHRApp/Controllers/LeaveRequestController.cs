using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OutOfOfficeHRApp.Data;
using OutOfOfficeHRApp.Models;

namespace OutOfOfficeHRApp.Controllers
{
	[Authorize]
	[Route("[controller]")]
	public class LeaveRequestController : Controller
	{
		private readonly OutOfOfficeContext _context;
		private readonly Utilities _utilities;
		public LeaveRequestController(OutOfOfficeContext context, Utilities utilities)
		{
			_context = context;
			_utilities = utilities;
		}

		[HttpGet]
		public async Task<IActionResult> GetLeaveRequests(int page = 1)
		{
			int pageSize = 25;
			int totalItems = await _context.LeaveRequest.CountAsync();

			ViewBag.TotalPages = (int)Math.Ceiling(totalItems / (double)pageSize);
			ViewBag.CurrentPage = page;
			ViewBag.PageSize = pageSize;

			var leaveRequests = await _context.LeaveRequest.Skip((page - 1) * pageSize)
										  .Take(pageSize).Include(lr => lr.Employee).Include(lr => lr.AbsenceReason).ToListAsync();
			var name = User.Identity.Name.ToLower();
			return View("Index", leaveRequests);
		}

		[HttpGet("{id}")]

		public async Task<IActionResult> GetLeaveDetails(int id)
		{
			var leaveRequest = await _context.LeaveRequest.Include(lr => lr.Employee).Include(lr => lr.AbsenceReason).FirstOrDefaultAsync(lr => lr.ID == id);
			if (leaveRequest == null)
			{
				NotFound();
				return RedirectToAction("Index");
			}
			return View("Details", leaveRequest);
		}

		[HttpGet("Create")]
		public IActionResult AddLeaveRequest()
		{
			ViewBag.Employee = _utilities.CreateSelectList(_context.Employee, "ID", "FullName");
			ViewBag.AbsenceReason = _utilities.CreateSelectList(_context.AbsenceReason, "ID", "Name");

			return View("Create");
		}

		[HttpPost("Create")]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> AddLeaveRequest(LeaveRequest leaveRequest)
		{
			leaveRequest.Status = Status.New;
			var employee = await _context.Employee.FirstOrDefaultAsync(e => e.ID == leaveRequest.EmployeeID);
			var absenceReason = await _context.AbsenceReason.FirstOrDefaultAsync(ar => ar.ID == leaveRequest.AbsenceReasonID);
			if (employee == null || absenceReason == null)
			{
				return NotFound();
			}
			leaveRequest.Employee = employee;
			leaveRequest.AbsenceReason = absenceReason;
			_context.LeaveRequest.Add(leaveRequest);
			await _context.SaveChangesAsync();
			return RedirectToAction("Index");
		}
		[HttpGet("Submit/{id}")]
		public async Task<IActionResult> Submit(int id)
		{
			var leaveRequest = await _context.LeaveRequest.Include(lr => lr.AbsenceReason).Include(lr => lr.Employee).FirstOrDefaultAsync(lr => lr.ID == id);
			return View("Submit", leaveRequest);
		}

		[HttpPost("Submit/{id}")]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> SubmitRequest(int id)
		{
			var leaveRequest = await _context.LeaveRequest.FirstOrDefaultAsync(lr => lr.ID == id);
			if (leaveRequest == null)
			{
				return NotFound();
			}
			leaveRequest.Status = Status.Submitted;
			var hrManager = await _context.Employee.Where(lr => lr.Position.Name == "HR Manager").Select(lr => lr.ID).ToListAsync();
			Random rnd = new Random();
			var random = rnd.Next(hrManager.Count);
			var approvalRequest = new ApprovalRequest
			{
				LeaveRequest = leaveRequest,
				EmployeeID = hrManager[random],
				Status = Status.New
			};
			_context.LeaveRequest.Update(leaveRequest);
			_context.ApprovalRequest.Add(approvalRequest);
			await _context.SaveChangesAsync();
			return RedirectToAction("Index");

		}

		[HttpGet("Cancel/{id}")]
		public async Task<IActionResult> Cancel(int id)
		{
			var leaveRequest = await _context.LeaveRequest.Include(lr => lr.AbsenceReason).Include(lr => lr.Employee).FirstOrDefaultAsync(lr => lr.ID == id);
			return View("Cancel", leaveRequest);
		}

		[HttpPost("Cancel/{id}")]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> CancelRequest(int id)
		{
			var leaveRequest = await _context.LeaveRequest.FirstOrDefaultAsync(lr => lr.ID == id);
			if (leaveRequest == null)
			{
				return NotFound();
			}
			leaveRequest.Status = Status.Cancelled;
			_context.LeaveRequest.Update(leaveRequest);
			await _context.SaveChangesAsync();
			return RedirectToAction("Index");
		}
	}
}
