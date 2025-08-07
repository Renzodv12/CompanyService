namespace CompanyService.Core.Models.Task
{
    public class CreateTaskRequest
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public string ColumnId { get; set; }
        public DateTime? DueDate { get; set; }
        public List<string> Labels { get; set; } = new();
        public List<string> AssigneeUserIds { get; set; } = new();
    }

}
