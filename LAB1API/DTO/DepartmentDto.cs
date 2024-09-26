namespace LAB1API.DTO
{
    public class DepartmentDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public List<string> EmpNames { get; set; } = new List<string>();
        public List<string> ProjectsNames { get; set; } = new List<string>();
    }
}
