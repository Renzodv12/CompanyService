namespace CompanyService.Core.Entities
{
    public class TaskLabel
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid TaskId { get; set; }
        public string Name { get; set; }
        public string Color { get; set; } = "#3B82F6"; // Blue default

        // Navegación
        public Task Task { get; set; }
    }
}
