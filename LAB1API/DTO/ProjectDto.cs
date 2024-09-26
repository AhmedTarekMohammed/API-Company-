namespace LAB1API.DTO
{
    public class ProjectDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string DeptName { get; set; }
        public List<string> EmpNames { get; set; } = new List<string>();
    }
}
