using Microsoft.EntityFrameworkCore;
using LAB1API.Model;

namespace LAB1API.Model
{
    public class ApplicationContext : DbContext
    {
        //public ApplicationContext()
        //{

        //}

        public ApplicationContext(DbContextOptions options) : base(options)
        { 
        }

        public DbSet<Employee> Employees { get; set; }
        public DbSet<Department> Departments { get; set; }
        public DbSet<Project> Projects { get; set; }
       
    }
}
