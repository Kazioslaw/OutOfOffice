﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OutOfOfficeHRApp.Data;
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
        public async Task<IActionResult> GetLeaveRequests(int page = 1)
        {
            int pageSize = 25;
            int totalItems = await _context.LeaveRequest.CountAsync();

            ViewBag.TotalPages = (int)Math.Ceiling(totalItems / (double)pageSize);
            ViewBag.CurrentPage = page;
            ViewBag.PageSize = pageSize;

            var leaveRequests = await _context.LeaveRequest.Skip((page - 1) * pageSize)
                                          .Take(pageSize).Include(lr => lr.Employee).Include(lr => lr.AbsenceReason).ToListAsync();
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
            ViewBag.Employee = CreateSelectList(_context.Employee, "ID", "FullName");
            ViewBag.AbsenceReason = CreateSelectList(_context.AbsenceReason, "ID", "Name");

            return View("Create");
        }

        [HttpPost("Create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddLeaveRequest(LeaveRequest leaveRequest)
        {
            leaveRequest.Status = Status.New;
            var employee = await _context.Employee.FindAsync(leaveRequest.EmployeeID);
            var absenceReason = await _context.AbsenceReason.FindAsync(leaveRequest.AbsenceReasonID);
            if (employee == null || absenceReason == null)
            {
                return NotFound();
            }
            leaveRequest.Employee = employee;
            leaveRequest.AbsenceReason = absenceReason;
            _context.Add(leaveRequest);
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
            var leaveRequest = await _context.LeaveRequest.FindAsync(id);
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
            _context.Update(leaveRequest);
            _context.Add(approvalRequest);
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
            var leaveRequest = await _context.LeaveRequest.FindAsync(id);
            if (leaveRequest == null)
            {
                return NotFound();
            }
            leaveRequest.Status = Status.Cancelled;
            _context.Update(leaveRequest);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }
    }
}
