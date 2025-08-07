namespace CompanyService.Core.Models.Task
{
    public class ColumnDto
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public List<string> TaskIds { get; set; } = new();
    }
}
