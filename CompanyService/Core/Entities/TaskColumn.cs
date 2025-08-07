namespace CompanyService.Core.Entities
{
    public class TaskColumn
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Name { get; set; }
        public int DisplayOrder { get; set; }
        public Guid CompanyId { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navegación
        public Company Company { get; set; }
        public ICollection<Task> Tasks { get; set; } = new List<Task>();
    }

}
