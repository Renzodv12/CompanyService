namespace CompanyService.Core.DTOs.Restaurant
{
    public class RestaurantDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string Address { get; set; } = string.Empty;
        public string? City { get; set; }
        public string? Phone { get; set; }
        public string? Email { get; set; }
        public string? RUC { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public Guid CompanyId { get; set; }
        public Guid CreatedBy { get; set; }
        public string CreatedByName { get; set; } = string.Empty;
        public int TotalTables { get; set; }
        public int AvailableTables { get; set; }
        public int TotalMenus { get; set; }
        public int ActiveOrders { get; set; }
    }
}