using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OutOfOfficeHRApp.Data;
using OutOfOfficeHRApp.Models;

namespace OutOfOfficeHRApp.Controllers
{
	[Authorize(Roles = "HR Manager, Project Manager, Admin")]
	[Route("[controller]")]
	public class EmployeeController : Controller
	{
		private readonly OutOfOfficeContext _context;
		private readonly IWebHostEnvironment _environment;
		private readonly UserManager<User> _userManager;
		private readonly Utilities _utilities;
		public EmployeeController(OutOfOfficeContext context, IWebHostEnvironment environment, UserManager<User> userManager, Utilities utilities)
		{
			_context = context;
			_environment = environment;
			_userManager = userManager;
			_utilities = utilities;
		}

		[HttpGet]
		public async Task<IActionResult> GetEmployee(int page = 1)
		{
			int pageSize = 25;
			int totalItems = await _context.Employee.CountAsync();

			ViewBag.TotalPages = (int)Math.Ceiling(totalItems / (double)pageSize);
			ViewBag.CurrentPage = page;
			ViewBag.PageSize = pageSize;

			var employees = await _context.Employee
										  .Skip((page - 1) * pageSize)
										  .Take(pageSize)
										  .Include(e => e.Subdivision)
										  .Include(e => e.Position)
										  .Include(e => e.PeoplePartner)
										  .ToListAsync();



			return View("Index", employees);
		}

		[HttpGet("{id}")]
		public async Task<IActionResult> GetEmployeeDetails(int id)
		{
			var employee = await _context.Employee
										 .Include(e => e.PeoplePartner)
										 .Include(e => e.Subdivision)
										 .Include(e => e.Position)
										 .Include(e => e.Project)
										 .FirstOrDefaultAsync(e => e.ID == id);
			if (employee == null)
			{
				NotFound();
				return RedirectToAction("Index");
			}
			return View("Details", employee);
		}


		[HttpGet("Create")]
		public IActionResult AddEmployee()
		{
			var employee = new Employee();
			ViewBag.Subdivision = _utilities.CreateSelectList(_context.Subdivision, "ID", "Name");
			ViewBag.Position = _utilities.CreateSelectList(_context.Position, "ID", "Name");
			ViewBag.PeoplePartner = _utilities.CreateSelectList(_context.Employee.Where(e => e.Position.Name == "HR Manager"), "ID", "FullName");
			ViewBag.Status = _utilities.CreateSelectList(new[] { new { Value = true, Text = "Active" }, new { Value = false, Text = "Inactive" } }, "Value", "Text");
			return View("Create", employee);
		}

		[HttpPost("Create")]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> AddEmployee(Employee employee)
		{
			ModelState.Remove("Position");
			ModelState.Remove("Subdivision");
			ModelState.Remove("PeoplePartner");
			ModelState.Remove("Project");
			ModelState.Remove("Email");
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

			var username = await _utilities.GenerateUsername(employee.FullName);
			var email = $"{username.Replace("_", "").ToLower()}@site.com";

			employee.PeoplePartner = await _context.Employee.FirstOrDefaultAsync(e => e.ID == employee.PeoplePartnerID);
			employee.Subdivision = await _context.Subdivision.FirstOrDefaultAsync(s => s.ID == employee.SubdivisionID);
			employee.Position = await _context.Position.FirstOrDefaultAsync(p => p.ID == employee.PositionID);
			employee.IsActive = true;
			employee.Email = email;

			var user = new User
			{
				UserName = username,
				Email = email,
				EmailConfirmed = true
			};
			var tempPassword = "P@ssword1";

			var userResult = await _userManager.CreateAsync(user, tempPassword);

			if (userResult.Succeeded)
			{
				if (employee.Position.Name == "Administrator")
				{
					var roleResult = await _userManager.AddToRoleAsync(user, "Admin");
				}
				else
				{
					var roleResult = await _userManager.AddToRoleAsync(user, $"{employee.Position.Name}");
				}

				if (!ModelState.IsValid)
				{
					foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
					{
						Console.WriteLine(error.ErrorMessage);
					}
				}

				if (ModelState.IsValid)
				{
					_context.Employee.Add(employee);
					await _context.SaveChangesAsync();
					return RedirectToAction(nameof(GetEmployee));
				}
			}

			else
			{
				foreach (var error in userResult.Errors)
				{
					ModelState.AddModelError("", error.Description);
				}
			}
			return View("Create");
		}

		[HttpGet("Edit/{id}")]
		public async Task<IActionResult> EditEmployee(int id)
		{
			var existingEmployee = await _context.Employee.Include(e => e.Subdivision).Include(e => e.Position).FirstOrDefaultAsync(e => e.ID == id);
			ViewBag.Subdivision = _utilities.CreateSelectList(_context.Subdivision, "ID", "Name");
			ViewBag.Position = _utilities.CreateSelectList(_context.Position, "ID", "Name");
			ViewBag.PeoplePartner = _utilities.CreateSelectList(_context.Employee.Where(e => e.Position.Name == "HR Manager"), "ID", "FullName");
			ViewBag.Status = _utilities.CreateSelectList(new[] { new { Value = true, Text = "Active" }, new { Value = false, Text = "Inactive" } }, "Value", "Text");
			return View("Edit", existingEmployee);
		}

		[HttpPost("Edit/{id}")]
		[ValidateAntiForgeryToken]
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

			var existingEmployeePosition = await _context.Position.Where(p => p.ID == existingEmployee.PositionID).Select(p => p.Name).FirstOrDefaultAsync();
			var employeePosition = await _context.Position.Where(p => p.ID == employee.PositionID).Select(p => p.Name).FirstOrDefaultAsync();
			existingEmployee.FullName = employee.FullName;
			existingEmployee.SubdivisionID = employee.SubdivisionID;
			existingEmployee.PositionID = employee.PositionID;
			existingEmployee.ProjectID = employee.ProjectID;
			existingEmployee.PeoplePartnerID = employee.PeoplePartnerID;
			existingEmployee.OutOfOfficeBalance = employee.OutOfOfficeBalance;
			var user = await _userManager.FindByEmailAsync(existingEmployee.Email);
			if (user != null && existingEmployeePosition != employeePosition)
			{
				var result = _userManager.RemoveFromRoleAsync(user, existingEmployee.PositionID.ToString());

				if (employeePosition == "Administrator")
				{
					var roleResult = await _userManager.AddToRoleAsync(user, "Admin");
				}
				else
				{
					var roleResult = await _userManager.AddToRoleAsync(user, $"{employeePosition}");
				}
			}

			if (ModelState.IsValid)
			{
				_context.Employee.Update(existingEmployee);
				await _context.SaveChangesAsync();
				return RedirectToAction(nameof(GetEmployee));
			}

			return View("Edit", existingEmployee);
		}


		[HttpPost("Activate/{id}")]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Activate(int id)
		{
			var employee = await _context.Employee.Include(e => e.Position).FirstOrDefaultAsync(e => e.ID == id);
			if (employee == null)
			{
				return NotFound();
			}
			var user = await _userManager.FindByEmailAsync(employee.Email);

			var tempPassword = "P@ssword1";
			if (user == null)
			{
				var tempUser = new User
				{
					UserName = await _utilities.GenerateUsername(employee.FullName),
					Email = employee.Email,
					EmailConfirmed = true
				};


				var userResult = await _userManager.CreateAsync(tempUser, tempPassword);
				if (userResult.Succeeded)
				{
					if (employee.Position.Name == "Administrator")
					{
						await _userManager.AddToRoleAsync(user, "Admin");
					}
					else
					{
						await _userManager.AddToRoleAsync(tempUser, employee.Position.Name);
					}
				}
			}
			employee.IsActive = true;

			_context.Employee.Update(employee);
			await _context.SaveChangesAsync();
			return RedirectToAction(nameof(GetEmployee));
		}


		[HttpPost("Deactivate/{id}")]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> DeactivateEmployee(int id)
		{
			var employee = await _context.Employee.FirstOrDefaultAsync(e => e.ID == id);
			if (employee == null)
			{
				return NotFound();
			}
			employee.IsActive = false;
			var user = await _userManager.FindByNameAsync(employee.FullName.Replace(" ", "_"));
			if (user != null)
			{
				await _userManager.DeleteAsync(user);
			}
			_context.Employee.Update(employee);
			await _context.SaveChangesAsync();
			return RedirectToAction(nameof(GetEmployee));
		}

		[HttpPost("Edit/{id}/Remove")]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> RemovePhoto(int id)
		{
			var employee = await _context.Employee.FirstOrDefaultAsync(e => e.ID == id);

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
				_context.Employee.Update(employee);
				await _context.SaveChangesAsync();
			}

			return Ok("Photo successfully removed");
		}

	}
}
