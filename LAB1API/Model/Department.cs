namespace LAB1API.Model
{
    public class Department
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public virtual List<Employee> Employees { get; set; }
        public virtual List<Project> Projects { get; set; }
    }
}
