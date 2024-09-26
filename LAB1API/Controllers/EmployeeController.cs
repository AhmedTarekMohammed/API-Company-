using Microsoft.AspNetCore.Mvc;
using LAB1API.DTO;
using LAB1API.Model;
using System;
using Microsoft.EntityFrameworkCore;


namespace LAB1API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EmployeeController : ControllerBase
    {
        private readonly ApplicationContext _context;

        public EmployeeController(ApplicationContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            try {
                var employees = _context.Employees
              .Include(d => d.Department)
              .Include(p => p.Projects)
              .ToList();
                var employeeDtos = employees.Select(e => new EmployeeDto
                {
                    Id = e.Id,
                    Name = e.Name,
                    Salary = e.Salary,
                    DeptName = e.Department?.Name ?? "",
                    ProjectsNames = e.Projects?.Select(p => p.Name).ToList() ?? new List<string>()
                }).ToList();

                return Ok(employeeDtos);

            }
            catch (Exception ex)
            {
                
                return StatusCode(500, "An error occurred while fetching employees. " + ex.Message);
            }

        }

        [HttpGet("{id:int}", Name = "GetEmployeeById")]
        public IActionResult GetById(int id)
        {
            var employee = _context.Employees.Include(d => d.Department)
                .Include(p => p.Projects)
                .FirstOrDefault(e => e.Id == id);
            if (employee == null)
                return NotFound("Employee not found");

            var employeeDto = new EmployeeDto
            {
                Id = employee.Id,
                Name = employee.Name,
                Salary = employee.Salary,
                DeptName = employee.Department?.Name,
                ProjectsNames = employee.Projects.Select(p => p.Name).ToList()
            };

            return Ok(employeeDto);
        }

        [HttpGet("name/{name}", Name = "GetEmployeeByName")]
        public IActionResult GetByName(string name)
        {
            var employee = _context.Employees.Include(d => d.Department).Include(p => p.Projects).FirstOrDefault(e => e.Name == name);
            if (employee == null)
                return NotFound("Employee not found");

            var employeeDto = new EmployeeDto
            {
                Id = employee.Id,
                Name = employee.Name,
                Salary = employee.Salary,
                DeptName = employee.Department?.Name,
                ProjectsNames = employee.Projects.Select(p => p.Name).ToList()
            };

            return Ok(employeeDto);
        }

        [HttpPost]
        public IActionResult Create([FromBody] EmployeeDto employeeDto)
        {
            var department = _context.Departments.FirstOrDefault(d => d.Name == employeeDto.DeptName);
            if (department == null)
                return BadRequest("Department does not exist");

            var employee = new Employee
            {
                Name = employeeDto.Name,
                Salary = employeeDto.Salary,
                Department = department,
                Projects = new List<Project>()
            };

            foreach (var projectName in employeeDto.ProjectsNames)
            {
                var project = _context.Projects.FirstOrDefault(p => p.Name == projectName);
                if (project == null)
                    return BadRequest($"Project {projectName} does not exist");

                employee.Projects.Add(project);
            }

            _context.Employees.Add(employee);
            _context.SaveChanges();

            return CreatedAtRoute("GetEmployeeById", new { id = employee.Id }, employeeDto);
        }

        [HttpPut("{id:int}")]
        public IActionResult Update(int id, [FromBody] EmployeeDto employeeDto)
        {
            var employee = _context.Employees.Include(e => e.Projects).FirstOrDefault(e => e.Id == id);
            if (employee == null)
                return NotFound("Employee not found");

            var department = _context.Departments.FirstOrDefault(d => d.Name == employeeDto.DeptName);
            if (department == null)
                return BadRequest("Department does not exist");

            employee.Name = employeeDto.Name;
            employee.Salary = employeeDto.Salary;
            employee.Department = department;

            // Clear the current project list before binding the new Project data
            employee.Projects.Clear();

            foreach (var projectName in employeeDto.ProjectsNames)
            {
                var project = _context.Projects.FirstOrDefault(p => p.Name == projectName);
                if (project == null)
                    return BadRequest($"Project {projectName} does not exist");

                employee.Projects.Add(project);
            }

            _context.SaveChanges();

            return Ok(employeeDto);
        }

        [HttpDelete("{id:int}")]
        public IActionResult Delete(int id)
        {
            var employee = _context.Employees.FirstOrDefault(e => e.Id == id);
            if (employee == null)
                return NotFound("Employee not found");

            _context.Employees.Remove(employee);
            _context.SaveChanges();

            return Ok();
        }

    }
}
