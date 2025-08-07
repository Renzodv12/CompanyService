namespace CompanyService.Core.Entities
{
    public class TaskComment
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid TaskId { get; set; }
        public Guid AuthorId { get; set; }
        public string AuthorName { get; set; }
        public string AuthorUsername { get; set; }
        public string AuthorAvatar { get; set; }
        public string Content { get; set; }
        public Guid? ParentCommentId { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navegación
        public Task Task { get; set; }
        public TaskComment ParentComment { get; set; }
        public ICollection<TaskComment> Replies { get; set; } = new List<TaskComment>();
    }
}
