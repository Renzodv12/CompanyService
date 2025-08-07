namespace CompanyService.Core.Models.Task
{
    public class TaskDto
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string ColumnId { get; set; }
        public AuthorDto Author { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? DueDate { get; set; }
        public bool Subscribed { get; set; }
        public List<string> Labels { get; set; } = new();
        public List<AssigneeDto> Assignees { get; set; } = new();
        public List<AttachmentDto> Attachments { get; set; } = new();
        public List<SubtaskDto> Subtasks { get; set; } = new();
        public List<CommentDto> Comments { get; set; } = new();
    }

}
