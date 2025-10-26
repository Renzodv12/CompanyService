namespace CompanyService.Core.DTOs.Restaurant
{
    public class RestaurantTableDto
    {
        public Guid Id { get; set; }
        public string TableNumber { get; set; } = string.Empty;
        public string? Name { get; set; }
        public int Capacity { get; set; }
        public string Status { get; set; } = string.Empty;
        public string? Location { get; set; }
        public string? Description { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public Guid RestaurantId { get; set; }
        public string RestaurantName { get; set; } = string.Empty;
        public Guid? CurrentOrderId { get; set; }
        public string? CurrentOrderNumber { get; set; }
        public int? CurrentGuests { get; set; }
    }
}