namespace CompanyService.Core.Entities
{
    public class TaskAttachment
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid TaskId { get; set; }
        public string Name { get; set; }
        public string Extension { get; set; }
        public string Url { get; set; }
        public string Size { get; set; }
        public DateTime UploadedAt { get; set; } = DateTime.UtcNow;

        // Navegación
        public Task Task { get; set; }
    }

}
