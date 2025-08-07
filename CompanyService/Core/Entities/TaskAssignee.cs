namespace CompanyService.Core.Entities
{
    public class TaskAssignee
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid TaskId { get; set; }
        public Guid UserId { get; set; }
        public string Name { get; set; }
        public string Username { get; set; }
        public string Avatar { get; set; }
        public DateTime AssignedAt { get; set; } = DateTime.UtcNow;

        // Navegación
        public Task Task { get; set; }
    }

}
