using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OutOfOfficeHRApp.Data;
using OutOfOfficeHRApp.Models;

namespace OutOfOfficeHRApp.Controllers
{
	[Authorize(Roles = "HR Manager, Project Manager, Admin")]
	[Route("[controller]")]
	public class ProjectController : Controller
	{
		private readonly OutOfOfficeContext _context;

		public ProjectController(OutOfOfficeContext context)
		{
			_context = context;
		}

		[HttpGet]
		public async Task<IActionResult> GetProjects(int page = 1)
		{
			int pageSize = 25;
			int totalItems = await _context.Project.CountAsync();

			ViewBag.TotalPages = (int)Math.Ceiling(totalItems / (double)pageSize);
			ViewBag.CurrentPage = page;
			ViewBag.PageSize = pageSize;

			var projects = await _context.Project
										 .Skip((page - 1) * pageSize)
										 .Take(pageSize)
										 .Include(p => p.ProjectManager)
										 .Include(p => p.ProjectType)
										 .ToListAsync();
			return View("Index", projects);
		}

		[HttpGet("{id}")]
		public async Task<IActionResult> GetProject(int id)
		{

			var project = await _context.Project
										.Include(p => p.ProjectManager)
										.Include(p => p.ProjectType)
										.Include(p => p.Employees)
										.FirstOrDefaultAsync(p => p.ID == id);

			if (project == null)
			{
				return NotFound();
			}
			return View("Details", project);
		}

		[HttpGet("Create")]

		public async Task<IActionResult> AddProject()
		{
			ViewBag.ProjectType = CreateSelectList(_context.ProjectType, "ID", "Name");
			ViewBag.ProjectManager = CreateSelectList(_context.Employee.Where(e => e.Position.Name == "Project Manager"), "ID", "FullName");
			return View("Create");
		}

		[HttpPost("Create")]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> AddProject(Project project)
		{
			project.IsActive = true;
			_context.Project.Add(project);
			await _context.SaveChangesAsync();
			return RedirectToAction("Index");
		}

		[HttpGet("Edit/{id}")]
		public async Task<IActionResult> UpdateProject(int id)
		{
			ViewBag.AllEmployees = CreateSelectList(_context.Employee, "ID", "FullName");
			ViewBag.ProjectManager = CreateSelectList(_context.Employee.Where(e => e.Position.Name == "Project Manager"), "ID", "FullName");
			ViewBag.ProjectType = CreateSelectList(_context.ProjectType, "ID", "Name");
			var project = await _context.Project
										.Include(p => p.ProjectManager)
										.Include(p => p.ProjectType)
										.Include(p => p.Employees)
										.FirstOrDefaultAsync(p => p.ID == id);
			return View("Edit", project);
		}

		[HttpPost("Edit/{id}")]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> UpdateProject(int id, Project project, List<int> selectedEmployees)
		{
			var existingProject = await _context.Project.FirstOrDefaultAsync(p => p.ID == id);
			if (existingProject == null)
			{
				return NotFound();
			}
			existingProject.Employees.Clear();
			if (selectedEmployees != null)
			{
				foreach (var empId in selectedEmployees)
				{
					var employee = await _context.Employee.FirstOrDefaultAsync(e => e.ID == empId);
					if (employee != null)
					{
						existingProject.Employees.Add(employee);
					}
				}
			}
			existingProject.ProjectTypeID = project.ProjectTypeID;
			existingProject.StartDate = project.StartDate;
			existingProject.EndDate = project.EndDate;
			existingProject.ProjectManagerID = project.ProjectManagerID;
			existingProject.Comment = project.Comment;
			_context.Project.Update(existingProject);
			return RedirectToAction("Index");
		}

		[HttpPost("Deactivate/{id}")]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> DeactivateProject(int id)
		{
			var project = await _context.Project.FirstOrDefaultAsync(p => p.ID == id);
			if (project == null)
			{
				return NotFound();
			}
			project.IsActive = false;
			_context.Project.Update(project);
			await _context.SaveChangesAsync();
			return RedirectToAction("Index");

		}

		[HttpPost("Activate/{id}")]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> ActivateProject(int id)
		{
			var project = await _context.Project.FirstOrDefaultAsync(p => p.ID == id);
			if (project == null)
			{
				return NotFound();
			}
			project.IsActive = true;
			_context.Project.Update(project);
			await _context.SaveChangesAsync();
			return RedirectToAction("Index");
		}
	}
}
