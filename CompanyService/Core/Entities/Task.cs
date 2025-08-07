namespace CompanyService.Core.Entities
{
    public class Task
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Title { get; set; }
        public string Description { get; set; }
        public Guid ColumnId { get; set; }
        public Guid AuthorId { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime LastModifiedAt { get; set; } = DateTime.UtcNow;
        public DateTime? DueDate { get; set; }
        public bool IsSubscribed { get; set; } = false;
        public Guid CompanyId { get; set; }

        // Navegación
        public TaskColumn Column { get; set; }
        public Company Company { get; set; }
        public ICollection<TaskLabel> TaskLabels { get; set; } = new List<TaskLabel>();
        public ICollection<TaskAssignee> TaskAssignees { get; set; } = new List<TaskAssignee>();
        public ICollection<TaskAttachment> TaskAttachments { get; set; } = new List<TaskAttachment>();
        public ICollection<TaskSubtask> TaskSubtasks { get; set; } = new List<TaskSubtask>();
        public ICollection<TaskComment> TaskComments { get; set; } = new List<TaskComment>();
    }
}
