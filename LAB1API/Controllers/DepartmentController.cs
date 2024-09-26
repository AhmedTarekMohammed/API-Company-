using LAB1API.DTO;
using LAB1API.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LAB1API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DepartmentController : ControllerBase
    {
        private readonly ApplicationContext _context;

        public DepartmentController(ApplicationContext context)
        {
            _context = context;
        }

        
        [HttpGet]
        public IActionResult GetAll()
        {
            var departments = _context.Departments.Include(e => e.Employees)
                .Include(p => p.Projects)
                .ToList();
            var departmentDtos = departments.Select(d => new DepartmentDto
            {
                Id = d.Id,
                Name = d.Name,
                EmpNames = d.Employees.Select(e => e.Name).ToList(),
                ProjectsNames = d.Projects.Select(p => p.Name).ToList()
            }).ToList();

            return Ok(departmentDtos);
        }

        [HttpGet("{id:int}", Name = "GetDepartmentById")]
        public IActionResult GetById(int id)
        {
            var department = _context.Departments.Include(e => e.Employees)
                .Include(p => p.Projects)
                .FirstOrDefault(d => d.Id == id);
            if (department == null)
                return NotFound("Department not found");

            var departmentDto = new DepartmentDto
            {
                Id = department.Id,
                Name = department.Name,
                EmpNames = department.Employees.Select(e => e.Name).ToList(),
                ProjectsNames = department.Projects.Select(p => p.Name).ToList()
            };

            return Ok(departmentDto);
        }

     
        [HttpPost]
        public IActionResult Create([FromBody] DepartmentDto departmentDto)
        {
            var department = new Department
            {
                Name = departmentDto.Name,
                Employees = new List<Employee>(),
                Projects = new List<Project>()
            };

            foreach (var empName in departmentDto.EmpNames)
            {
                var employee = _context.Employees.FirstOrDefault(e => e.Name == empName);
                if (employee == null) return BadRequest($"{empName} does not exist in our employee list");
                department.Employees.Add(employee);
            }

            foreach (var projectName in departmentDto.ProjectsNames)
            {
                var project = _context.Projects.FirstOrDefault(p => p.Name == projectName);
                if (project == null) return BadRequest($"{projectName} does not exist in our project list");
                department.Projects.Add(project);
            }

            _context.Departments.Add(department);
            _context.SaveChanges();

            return CreatedAtRoute("GetDepartmentById", new { id = department.Id }, departmentDto);
        }


        [HttpPut("{id:int}")]
        public IActionResult Update(int id, [FromBody] DepartmentDto departmentDto)
        {
            var department = _context.Departments.Include(e => e.Employees)
                .Include(p => p.Projects)
                .FirstOrDefault(d => d.Id == id);
            if (department == null)
                return NotFound("Department not found");

            department.Name = departmentDto.Name;

            //department.Employees.Clear();
            foreach (var empName in departmentDto.EmpNames)
            {
                var employee = _context.Employees.FirstOrDefault(e => e.Name == empName);
                if (employee == null) return BadRequest($"{empName} does not exist in our employee list");
                department.Employees.Add(employee);
            }

            //department.Projects.Clear();
            foreach (var projectName in departmentDto.ProjectsNames)
            {
                var project = _context.Projects.FirstOrDefault(p => p.Name == projectName);
                if (project == null) return BadRequest($"{projectName} does not exist in our project list");
                department.Projects.Add(project);
            }

            _context.SaveChanges();
            return Ok(departmentDto);
        }

        ///

        /// --/////// Don't use i'm trying to debuggg
        //[HttpPut("{id:int}")]
        //public IActionResult Update(int id, DepartmentDto departmentDto)
        //{
        //    var department = _context.Departments
        //        .Include(e => e.Employees)
        //        .Include(p => p.Projects)
        //        .FirstOrDefault(d => d.Id == id);

        //    if (department == null)
        //        return NotFound("Department not found");

        //    // Update Name only if it's provided
        //    if (!string.IsNullOrEmpty(departmentDto.Name))
        //    {
        //        department.Name = departmentDto.Name;
        //    }

        //    // Update Employees only if the list is provided
        //    if (departmentDto.EmpNames != null && departmentDto.EmpNames.Any())
        //    {
        //        department.Employees.Clear();
        //        foreach (var empName in departmentDto.EmpNames)
        //        {
        //            var employee = _context.Employees.FirstOrDefault(e => e.Name == empName);
        //            if (employee == null)
        //                return BadRequest($"{empName} does not exist in our employee list");
        //            department.Employees.Add(employee);
        //        }
        //    }

        //    // Update Projects only if the list is provided
        //    if (departmentDto.ProjectsNames != null && departmentDto.ProjectsNames.Any())
        //    {
        //        department.Projects.Clear();
        //        foreach (var projectName in departmentDto.ProjectsNames)
        //        {
        //            var project = _context.Projects.FirstOrDefault(p => p.Name == projectName);
        //            if (project == null)
        //                return BadRequest($"{projectName} does not exist in our project list");
        //            department.Projects.Add(project);
        //        }
        //    }

        //    _context.SaveChanges();
        //    return Ok(department);
        //}

        [HttpDelete("{id:int}")]
        public IActionResult Delete(int id)
        {
            var department = _context.Departments.Include(e => e.Employees)
                .Include(p => p.Projects)
                .FirstOrDefault(d => d.Id == id);
            if (department == null)
                return NotFound("Department not found");

            if (department.Employees.Any() || department.Projects.Any())
                return BadRequest("Cannot delete department because it has employees or projects assigned to it");

            _context.Departments.Remove(department);
            _context.SaveChanges();
            return Ok();
        }
    }
}
