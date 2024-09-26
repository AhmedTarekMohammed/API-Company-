using System.ComponentModel.DataAnnotations.Schema;

namespace LAB1API.Model
{
    public class Project
    {
        public int Id { get; set; }
        public string Name { get; set; }

        [ForeignKey("Department")]
        public int DeptId { get; set; }
        public virtual Department Department { get; set; }
        public virtual List<Employee> Employees { get; set; }
    }
}
