using MediatR;
using CompanyService.Core.DTOs.Restaurant;

namespace CompanyService.Core.Feature.Querys.Restaurant
{
    public class GetRestaurantOrdersQuery : IRequest<List<RestaurantOrderDto>>
    {
        public Guid RestaurantId { get; set; }
        public Guid CompanyId { get; set; }
        public int? Status { get; set; }
        public int? Type { get; set; }
        public Guid? TableId { get; set; }
        public int PageSize { get; set; } = 50;
    }

    public class GetRestaurantOrderByIdQuery : IRequest<RestaurantOrderDto?>
    {
        public Guid Id { get; set; }
        public Guid RestaurantId { get; set; }
        public Guid CompanyId { get; set; }
    }

    public class GetActiveOrdersQuery : IRequest<List<RestaurantOrderDto>>
    {
        public Guid RestaurantId { get; set; }
        public Guid CompanyId { get; set; }
    }

    public class GetOrdersByTableQuery : IRequest<List<RestaurantOrderDto>>
    {
        public Guid TableId { get; set; }
        public Guid RestaurantId { get; set; }
        public Guid CompanyId { get; set; }
    }

    public class GetRestaurantPaymentsQuery : IRequest<List<RestaurantPaymentDto>>
    {
        public Guid OrderId { get; set; }
        public Guid RestaurantId { get; set; }
        public Guid CompanyId { get; set; }
    }

    public class GetRestaurantMenuQuery : IRequest<RestaurantMenuDto?>
    {
        public Guid RestaurantId { get; set; }
        public Guid CompanyId { get; set; }
    }

    public class GetRestaurantMenuWithItemsQuery : IRequest<RestaurantMenuWithItemsDto?>
    {
        public Guid RestaurantId { get; set; }
        public Guid CompanyId { get; set; }
        public string? Category { get; set; }
        public bool? IsAvailable { get; set; }
        public bool? IsVegetarian { get; set; }
        public bool? IsVegan { get; set; }
        public bool? IsGlutenFree { get; set; }
    }

    public class GetMenuCategoriesQuery : IRequest<List<string>>
    {
        public Guid RestaurantId { get; set; }
        public Guid CompanyId { get; set; }
    }

    public class GetRestaurantReportsQuery : IRequest<RestaurantReportsDto>
    {
        public Guid RestaurantId { get; set; }
        public Guid CompanyId { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
    }

    public class GetRestaurantStatsQuery : IRequest<RestaurantStatsDto>
    {
        public Guid RestaurantId { get; set; }
        public Guid CompanyId { get; set; }
    }

    public class GetTableStatusQuery : IRequest<List<TableStatusDto>>
    {
        public Guid RestaurantId { get; set; }
        public Guid CompanyId { get; set; }
    }
}