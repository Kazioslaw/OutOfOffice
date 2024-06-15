﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using OutOfOfficeHRApp.Data;
using OutOfOfficeHRApp.Models;

namespace OutOfOfficeHRApp.Controllers
{
    [Route("[controller]")]
    public class EmployeeController : Controller
    {
        private readonly OutOfOfficeContext _context;
        private readonly IWebHostEnvironment _environment;
        public EmployeeController(OutOfOfficeContext context, IWebHostEnvironment environment)
        {
            _context = context;
            _environment = environment;
        }

        [HttpGet]
        public async Task<IActionResult> GetEmployee(string sortOrder)
        {
            ViewData["FullNameSortParam"] = sortOrder == "FullName_asc" ? "FullName_desc" : "FullName_asc";
            ViewData["SubdivisionSortParam"] = sortOrder == "Subdivision_asc" ? "Subdivision_desc" : "Subdivision_asc";
            ViewData["PositionSortParam"] = sortOrder == "Position_asc" ? "Position_desc" : "Position_asc";
            ViewData["IsActiveSortParam"] = sortOrder == "IsActive_asc" ? "IsActive_desc" : "IsActive_asc";
            ViewData["PeoplePartnerIDSortParam"] = sortOrder == "PeoplePartnerID_asc" ? "PeoplePartnerID_desc" : "PeoplePartnerID_asc";
            ViewData["OutOfOfficeBalaceSortParam"] = sortOrder == "OutOfOfficeBalace_asc" ? "OutOfOfficeBalace_desc" : "OutOfOfficeBalace_asc";

            IEnumerable<Employee> employees = await _context.Employee
                      .Include(e => e.Subdivision)
                      .Include(e => e.Position).ToListAsync();

            switch (sortOrder)
            {
                case "FullName_asc":
                    employees = employees.OrderBy(e => e.FullName);
                    break;
                case "FullName_desc":
                    employees = employees.OrderByDescending(e => e.FullName);
                    break;
                case "Subdivision_asc":
                    employees = employees.OrderBy(e => e.Subdivision.Name);
                    break;
                case "Subdivision_desc":
                    employees = employees.OrderByDescending(e => e.Subdivision.Name);
                    break;
                case "Position_asc":
                    employees = employees.OrderBy(e => e.Position.Name);
                    break;
                case "Position_desc":
                    employees = employees.OrderByDescending(e => e.Position.Name);
                    break;
                case "IsActive_asc":
                    employees = employees.OrderBy(e => e.IsActive);
                    break;
                case "IsActive_desc":
                    employees = employees.OrderByDescending(e => e.IsActive);
                    break;
                case "PeoplePartner_asc":
                    employees = employees.OrderBy(e => e.PeoplePartner.FullName);
                    break;
                case "PeoplePartner_desc":
                    employees = employees.OrderByDescending(e => e.PeoplePartner.FullName);
                    break;
                case "OutOfOfficeBalance_asc":
                    employees = employees.OrderBy(e => e.OutOfOfficeBalance);
                    break;
                case "OutOfOfficeBalance_desc":
                    employees = employees.OrderByDescending(e => e.OutOfOfficeBalance);
                    break;
                default:
                    employees = employees.OrderBy(e => e.ID);
                    break;

            }


            return View("Index", employees);
        }

        [HttpGet("Create")]
        public IActionResult AddEmployee()
        {
            var employee = new Employee();
            ViewBag.SubdivisionID = new SelectList(_context.Subdivision, "ID", "Name");
            ViewBag.PositionID = new SelectList(_context.Position, "ID", "Name");
            ViewBag.PeoplePartnerID = new SelectList(_context.Employee.Where(e => e.Position.Name == "HR Manager"), "ID", "FullName");
            ViewBag.Status = new SelectList(new[] { new { Value = true, Text = "Active" }, new { Value = false, Text = "Inactive" } }, "Value", "Text");
            return View("Create", employee);
        }

        [HttpPost("Create")]
        public async Task<IActionResult> AddEmployee(Employee employee)
        {
            if (employee.Photo != null && employee.Photo.Length > 0)
            {
                var uploadDirectory = Path.Combine(_environment.WebRootPath, "images", "EmployeePhotos");
                if (!Directory.Exists(uploadDirectory))
                {
                    Directory.CreateDirectory(uploadDirectory);
                }
                var fileExtension = Path.GetExtension(employee.Photo.FileName);
                var fileName = $"{employee.FullName.Replace(" ", "_")}_{DateTime.Now.ToString("yyyy-MM-dd_hh-mm-ss")}{fileExtension}";
                var filePath = Path.Combine(uploadDirectory, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await employee.Photo.CopyToAsync(stream);
                }
                employee.PhotoPath = "/images/EmployeePhotos/" + fileName;
            }

            var partnerID = employee.PeoplePartnerID;
            var subdivisionID = employee.SubdivisionID;
            var positionID = employee.PositionID;

            employee.PeoplePartner = await _context.Employee.FindAsync(partnerID);
            employee.Subdivision = await _context.Subdivision.FindAsync(subdivisionID);
            employee.Position = await _context.Position.FindAsync(positionID);

            ModelState.Remove("Position");
            ModelState.Remove("Subdivision");
            ModelState.Remove("PeoplePartner");

            if (ModelState.IsValid)
            {
                _context.Add(employee);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(GetEmployee));
            }
            return View("Create", employee);
        }

        [HttpGet("Edit/{id}")]
        public async Task<IActionResult> EditEmployee(int id)
        {
            var existingEmployee = await _context.Employee.Include(e => e.Subdivision).Include(e => e.Position).FirstOrDefaultAsync(e => e.ID == id);
            ViewBag.SubdivisionID = new SelectList(_context.Subdivision, "ID", "Name");
            ViewBag.PositionID = new SelectList(_context.Position, "ID", "Name");
            ViewBag.PeoplePartnerID = new SelectList(_context.Employee.Where(e => e.Position.Name == "HR Manager"), "ID", "FullName");
            ViewBag.Status = new SelectList(new[] { new { Value = true, Text = "Active" }, new { Value = false, Text = "Inactive" } }, "Value", "Text");
            return View("Edit", existingEmployee);
        }

        [HttpPost("Edit/{id}")]
        public async Task<IActionResult> EditEmployee(int id, Employee employee)
        {
            var existingEmployee = _context.Employee.FirstOrDefault(e => e.ID == id);
            if (id != employee.ID)
            {
                return BadRequest();
            }

            if (existingEmployee == null)
            {
                return NotFound();
            }

            if (employee.FullName == existingEmployee.FullName ||
               employee.SubdivisionID == existingEmployee.SubdivisionID ||
               employee.PositionID == existingEmployee.PositionID ||
               employee.IsActive == existingEmployee.IsActive ||
               employee.Photo == null)
            {
                ModelState.Clear();
            }

            if (employee.Photo != null && employee.Photo.Length > 0)
            {
                var uploadDirectory = Path.Combine(_environment.WebRootPath, "images", "EmployeePhotos");
                if (!Directory.Exists(uploadDirectory))
                {
                    Directory.CreateDirectory(uploadDirectory);
                }

                var fileExtension = Path.GetExtension(employee.Photo.FileName);
                var fileName = $"{employee.FullName.Replace(" ", "_")}_{DateTime.Now.ToString("yyyy-MM-dd_hh-mm-ss")}{fileExtension}";
                var filePath = Path.Combine(uploadDirectory, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await employee.Photo.CopyToAsync(stream);
                }

                existingEmployee.PhotoPath = "/images/EmployeePhotos/" + fileName;
            }

            existingEmployee.FullName = employee.FullName;
            existingEmployee.SubdivisionID = employee.SubdivisionID;
            existingEmployee.PositionID = employee.PositionID;
            existingEmployee.IsActive = employee.IsActive;
            existingEmployee.OutOfOfficeBalance = employee.OutOfOfficeBalance;


            if (ModelState.IsValid)
            {
                _context.Employee.Update(existingEmployee);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(GetEmployee));
            }

            ViewBag.SubdivisionID = new SelectList(_context.Subdivision, "ID", "Name", existingEmployee.SubdivisionID);
            ViewBag.PositionID = new SelectList(_context.Position, "ID", "Name", existingEmployee.PositionID);
            ViewBag.PeoplePartnerID = new SelectList(_context.Employee.Where(e => e.Position.Name == "HR Manager"), "ID", "FullName", existingEmployee.PeoplePartnerID);
            ViewBag.Status = new SelectList(new[] { new { Value = true, Text = "Active" }, new { Value = false, Text = "Inactive" } }, "Value", "Text", existingEmployee.IsActive);
            return View("Edit", existingEmployee);
        }


        [HttpGet("Deactivate/{id}")]

        public IActionResult Deactivate(int id)
        {
            var employee = _context.Employee.Include(e => e.Subdivision).Include(e => e.Position).Include(e => e.PeoplePartner).FirstOrDefault(e => e.ID == id);
            return View("Deactivate", employee);
        }

        [HttpPost("Deactivate/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeactivateEmployee(int id)
        {
            var employee = _context.Employee.Include(e => e.Subdivision).Include(e => e.Position).Include(e => e.PeoplePartner).FirstOrDefault();
            if (employee == null)
            {
                return NotFound();
            }
            employee.IsActive = false;
            _context.Employee.Update(employee);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(GetEmployee));
        }

        [HttpPost("Edit/{id}/RemovePhoto")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RemovePhoto(int id)
        {
            var employee = await _context.Employee.FindAsync(id);

            if (employee == null)
            {
                return NotFound();
            }

            if (!string.IsNullOrEmpty(employee.PhotoPath))
            {

                var fullPath = Path.Combine(_environment.WebRootPath, employee.PhotoPath.TrimStart('/'));
                if (System.IO.File.Exists(fullPath))
                {
                    System.IO.File.Delete(fullPath);
                }

                employee.PhotoPath = null;
                _context.Update(employee);
                await _context.SaveChangesAsync();
            }

            return Ok("Photo successfully removed");
        }
    }
}