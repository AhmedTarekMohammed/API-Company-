namespace  LAB1API.DTO
{
    public class EmployeeDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public double Salary { get; set; }
        public string DeptName { get; set; }
        public List<string> ProjectsNames { get; set; } = new List<string>();
    }
}
