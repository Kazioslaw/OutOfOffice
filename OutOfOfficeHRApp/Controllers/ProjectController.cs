using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OutOfOfficeHRApp.Data;
using OutOfOfficeHRApp.Models;

namespace OutOfOfficeHRApp.Controllers
{
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
            var projects = await _context.Project.Include(p => p.ProjectManager).Include(p => p.ProjectType).ToListAsync();
            return View("Index", projects);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetProject(int id)
        {
            var project = await _context.Project.FindAsync(id);
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
        public async Task<IActionResult> AddProject(Project project)
        {
            project.IsActive = true;
            _context.Project.Add(project);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        [HttpPost("{id}")]
        public async Task<IActionResult> UpdateProject(int id, Project project)
        {
            var existingProject = await _context.Project.FindAsync(id);
            if (existingProject == null)
            {
                return NotFound();
            }
            existingProject.ProjectTypeID = project.ProjectTypeID;
            existingProject.StartDate = project.StartDate;
            existingProject.EndDate = project.EndDate;
            existingProject.EmployeeID = project.EmployeeID;
            existingProject.Comment = project.Comment;
            _context.Update(existingProject);
            return Ok();
        }

        [HttpPut("{id}/Deactivate")]
        public async Task<IActionResult> DeactivateProject(int id)
        {
            var project = await _context.Project.FindAsync(id);
            if (project == null)
            {
                return NotFound();
            }
            project.IsActive = false;
            _context.Update(project);
            await _context.SaveChangesAsync();
            return Ok();

        }
    }
}
