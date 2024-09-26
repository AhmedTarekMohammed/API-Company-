using Microsoft.AspNetCore.Mvc;
using LAB1API.DTO;
using LAB1API.Model;
using Microsoft.EntityFrameworkCore;


namespace LAB1API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProjectController : ControllerBase
    {
        private readonly ApplicationContext _context;

        public ProjectController(ApplicationContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            var projects = _context.Projects.Include(p => p.Department)
                .Include(p => p.Employees)
                .ToList();
            var projectDtos = projects.Select(p => new ProjectDto
            {
                Id = p.Id,
                Name = p.Name,
                DeptName = p.Department.Name,
                EmpNames = p.Employees.Select(e => e.Name).ToList()
            }).ToList();

            return Ok(projectDtos);
        }

        [HttpGet("{id:int}", Name = "GetProjectById")]
        public IActionResult GetById(int id)
        {
            var project = _context.Projects.Include(p => p.Department)
                .Include(p => p.Employees)
                 .FirstOrDefault(p => p.Id == id);

            if (project == null)
                return NotFound("Project not found");

            var projectDto = new ProjectDto
            {
                Id = project.Id,
                Name = project.Name,
                DeptName = project.Department.Name,
                EmpNames = project.Employees.Select(e => e.Name).ToList()
            };

            return Ok(projectDto);
        }

        [HttpGet("name/{name}", Name = "GetProjectByName")]
        public IActionResult GetByName(string name)
        {
            var project = _context.Projects.Include(p => p.Department).Include(p => p.Employees)
                                           .FirstOrDefault(p => p.Name == name);

            if (project == null)
                return NotFound("Project not found");

            var projectDto = new ProjectDto
            {
                Id = project.Id,
                Name = project.Name,
                DeptName = project.Department.Name,
                EmpNames = project.Employees.Select(e => e.Name).ToList()
            };

            return Ok(projectDto);
        }

        [HttpPost]
        public IActionResult Create([FromBody] ProjectDto projectDto)
        {
            try
            {
                var department = _context.Departments.FirstOrDefault(d => d.Name == projectDto.DeptName);

                if (department == null)
                    return BadRequest("The Department does not exist");

                var project = new Project
                {
                    Name = projectDto.Name,
                    Department = department,
                    DeptId = department.Id,
                    Employees = new List<Employee>()
                };

                foreach (var empName in projectDto.EmpNames)
                {
                    var employee = _context.Employees.FirstOrDefault(e => e.Name == empName);
                    if (employee == null)
                        return BadRequest($"Employee '{empName}' does not exist");

                    project.Employees.Add(employee);
                }

                _context.Projects.Add(project);
                _context.SaveChanges();

                return CreatedAtRoute("GetProjectById", new { id = project.Id }, projectDto);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("{id}")]
        public IActionResult Update(int id, [FromBody] ProjectDto projectDto)
        {
            var project = _context.Projects.Include(p => p.Employees).FirstOrDefault(p => p.Id == id);

            if (project == null)
                return NotFound("Project not found");

            var department = _context.Departments.FirstOrDefault(d => d.Name == projectDto.DeptName);

            if (department == null)
                return BadRequest("The Department does not exist");

            project.Name = projectDto.Name;
            project.Department = department;
            project.DeptId = department.Id;

            // Clear existing employees and reassign them
            project.Employees.Clear();
            foreach (var empName in projectDto.EmpNames)
            {
                var employee = _context.Employees.FirstOrDefault(e => e.Name == empName);
                if (employee == null)
                    return BadRequest($"Employee '{empName}' does not exist");

                project.Employees.Add(employee);
            }

            _context.SaveChanges();

            return Ok(projectDto);
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var project = _context.Projects.FirstOrDefault(p => p.Id == id);

            if (project == null)
                return NotFound("Project not found");

            _context.Projects.Remove(project);
            _context.SaveChanges();

            return Ok();
        }
    }

}
