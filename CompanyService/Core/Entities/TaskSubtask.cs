namespace CompanyService.Core.Entities
{
    public class TaskSubtask
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid TaskId { get; set; }
        public string Title { get; set; }
        public bool IsCompleted { get; set; } = false;
        public int DisplayOrder { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navegación
        public Task Task { get; set; }
    }
}
