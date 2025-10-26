using CompanyService.Core.Entities;
using CompanyService.Core.Entities.Restaurant;
using CompanyService.Core.Interfaces;
using CompanyService.Core.Services;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace CompanyService.Core.Services
{
    public interface IRestaurantService
    {
        Task<RestaurantOrder> CreateRestaurantOrderAsync(CreateRestaurantOrderRequest request);
        Task<RestaurantOrder> UpdateRestaurantOrderStatusAsync(Guid orderId, OrderStatus status);
        Task<RestaurantOrder> AssignOrderToTableAsync(Guid orderId, Guid tableId);
        Task<RestaurantOrder> AssignOrderToWaiterAsync(Guid orderId, Guid waiterId);
        Task<RestaurantOrder> CompleteOrderAsync(Guid orderId);
        Task<RestaurantOrder> CancelOrderAsync(Guid orderId);
        Task<IEnumerable<RestaurantOrder>> GetActiveOrdersAsync(Guid restaurantId);
        Task<IEnumerable<RestaurantOrder>> GetOrdersByTableAsync(Guid tableId);
        Task<IEnumerable<RestaurantOrder>> GetOrdersByWaiterAsync(Guid waiterId);
        Task<RestaurantOrder?> GetOrderByIdAsync(Guid orderId);
    }

    public class RestaurantService : IRestaurantService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<RestaurantService> _logger;

        public RestaurantService(
            IUnitOfWork unitOfWork, 
            ILogger<RestaurantService> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<RestaurantOrder> CreateRestaurantOrderAsync(CreateRestaurantOrderRequest request)
        {
            _logger.LogInformation("Creating restaurant order for table {TableId}", request.TableId);

            // Create the Sale directly
            var sale = new CompanyService.Core.Entities.Sale
            {
                Id = Guid.NewGuid(),
                SaleNumber = GenerateOrderNumber(),
                CustomerId = request.CustomerId ?? Guid.Empty,
                CompanyId = request.CompanyId,
                UserId = request.UserId,
                SaleDate = DateTime.UtcNow,
                Subtotal = request.MenuItems.Sum(mi => mi.Quantity * mi.UnitPrice),
                TotalAmount = request.MenuItems.Sum(mi => mi.Quantity * mi.UnitPrice),
                Notes = request.Notes,
                CreatedAt = DateTime.UtcNow
            };

            await _unitOfWork.Repository<CompanyService.Core.Entities.Sale>().AddAsync(sale);

            // Create SaleDetails
            foreach (var menuItem in request.MenuItems)
            {
                var saleDetail = new CompanyService.Core.Entities.SaleDetail
                {
                    Id = Guid.NewGuid(),
                    SaleId = sale.Id,
                    ProductId = menuItem.ProductId,
                    Quantity = menuItem.Quantity,
                    UnitPrice = menuItem.UnitPrice,
                    Subtotal = menuItem.Quantity * menuItem.UnitPrice
                };
                await _unitOfWork.Repository<CompanyService.Core.Entities.SaleDetail>().AddAsync(saleDetail);
            }

            // Then create the RestaurantOrder linked to the Sale
            var restaurantOrder = new RestaurantOrder
            {
                Id = Guid.NewGuid(),
                OrderNumber = GenerateOrderNumber(),
                Status = OrderStatus.Pending,
                Type = request.Type,
                CustomerName = request.CustomerName,
                CustomerPhone = request.CustomerPhone,
                NumberOfGuests = request.NumberOfGuests,
                Notes = request.Notes,
                SpecialInstructions = request.SpecialInstructions,
                OrderTime = DateTime.UtcNow,
                RestaurantId = request.RestaurantId,
                TableId = request.TableId,
                SaleId = sale.Id,
                CreatedBy = request.UserId,
                AssignedWaiterId = request.AssignedWaiterId,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            await _unitOfWork.Repository<RestaurantOrder>().AddAsync(restaurantOrder);
            await _unitOfWork.SaveChangesAsync();

            // Update table status
            await UpdateTableStatusAsync(request.TableId, TableStatus.Occupied, restaurantOrder.Id);

            _logger.LogInformation("Restaurant order created successfully with ID: {OrderId}", restaurantOrder.Id);
            return restaurantOrder;
        }

        public async Task<RestaurantOrder> UpdateRestaurantOrderStatusAsync(Guid orderId, OrderStatus status)
        {
            _logger.LogInformation("Updating restaurant order {OrderId} status to {Status}", orderId, status);

            var orders = await _unitOfWork.Repository<RestaurantOrder>()
                .WhereAsync(o => o.Id == orderId);

            var order = orders.FirstOrDefault();
            if (order == null)
            {
                throw new ArgumentException($"Restaurant order with ID {orderId} not found");
            }

            order.Status = status;
            order.UpdatedAt = DateTime.UtcNow;

            if (status == OrderStatus.Completed)
            {
                order.CompletedTime = DateTime.UtcNow;
                // Update table status to available
                await UpdateTableStatusAsync(order.TableId, TableStatus.Available, null);
            }

            _unitOfWork.Repository<RestaurantOrder>().Update(order);
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("Restaurant order status updated successfully");
            return order;
        }

        public async Task<RestaurantOrder> AssignOrderToTableAsync(Guid orderId, Guid tableId)
        {
            _logger.LogInformation("Assigning restaurant order {OrderId} to table {TableId}", orderId, tableId);

            var orders = await _unitOfWork.Repository<RestaurantOrder>()
                .WhereAsync(o => o.Id == orderId);

            var order = orders.FirstOrDefault();
            if (order == null)
            {
                throw new ArgumentException($"Restaurant order with ID {orderId} not found");
            }

            // Check if table is available
            var tables = await _unitOfWork.Repository<RestaurantTable>()
                .WhereAsync(t => t.Id == tableId);

            var table = tables.FirstOrDefault();
            if (table == null)
            {
                throw new ArgumentException($"Table with ID {tableId} not found");
            }

            if (table.Status != TableStatus.Available)
            {
                throw new InvalidOperationException($"Table {tableId} is not available");
            }

            order.TableId = tableId;
            order.UpdatedAt = DateTime.UtcNow;

            _unitOfWork.Repository<RestaurantOrder>().Update(order);
            await _unitOfWork.SaveChangesAsync();

            // Update table status
            await UpdateTableStatusAsync(tableId, TableStatus.Occupied, orderId);

            _logger.LogInformation("Restaurant order assigned to table successfully");
            return order;
        }

        public async Task<RestaurantOrder> AssignOrderToWaiterAsync(Guid orderId, Guid waiterId)
        {
            _logger.LogInformation("Assigning restaurant order {OrderId} to waiter {WaiterId}", orderId, waiterId);

            var orders = await _unitOfWork.Repository<RestaurantOrder>()
                .WhereAsync(o => o.Id == orderId);

            var order = orders.FirstOrDefault();
            if (order == null)
            {
                throw new ArgumentException($"Restaurant order with ID {orderId} not found");
            }

            order.AssignedWaiterId = waiterId;
            order.UpdatedAt = DateTime.UtcNow;

            _unitOfWork.Repository<RestaurantOrder>().Update(order);
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("Restaurant order assigned to waiter successfully");
            return order;
        }

        public async Task<RestaurantOrder> CompleteOrderAsync(Guid orderId)
        {
            return await UpdateRestaurantOrderStatusAsync(orderId, OrderStatus.Completed);
        }

        public async Task<RestaurantOrder> CancelOrderAsync(Guid orderId)
        {
            _logger.LogInformation("Cancelling restaurant order {OrderId}", orderId);

            var orders = await _unitOfWork.Repository<RestaurantOrder>()
                .WhereAsync(o => o.Id == orderId);

            var order = orders.FirstOrDefault();
            if (order == null)
            {
                throw new ArgumentException($"Restaurant order with ID {orderId} not found");
            }

            // Update order status
            order.Status = OrderStatus.Cancelled;
            order.UpdatedAt = DateTime.UtcNow;

            _unitOfWork.Repository<RestaurantOrder>().Update(order);
            await _unitOfWork.SaveChangesAsync();

            // Update table status to available
            await UpdateTableStatusAsync(order.TableId, TableStatus.Available, null);

            _logger.LogInformation("Restaurant order cancelled successfully");
            return order;
        }

        public async Task<IEnumerable<RestaurantOrder>> GetActiveOrdersAsync(Guid restaurantId)
        {
            var orders = await _unitOfWork.Repository<RestaurantOrder>()
                .WhereAsync(o => o.RestaurantId == restaurantId && 
                               (o.Status == OrderStatus.Pending || 
                                o.Status == OrderStatus.Confirmed || 
                                o.Status == OrderStatus.Preparing || 
                                o.Status == OrderStatus.Ready || 
                                o.Status == OrderStatus.Served));

            return orders.OrderByDescending(o => o.OrderTime);
        }

        public async Task<IEnumerable<RestaurantOrder>> GetOrdersByTableAsync(Guid tableId)
        {
            var orders = await _unitOfWork.Repository<RestaurantOrder>()
                .WhereAsync(o => o.TableId == tableId);

            return orders.OrderByDescending(o => o.OrderTime);
        }

        public async Task<IEnumerable<RestaurantOrder>> GetOrdersByWaiterAsync(Guid waiterId)
        {
            var orders = await _unitOfWork.Repository<RestaurantOrder>()
                .WhereAsync(o => o.AssignedWaiterId == waiterId);

            return orders.OrderByDescending(o => o.OrderTime);
        }

        public async Task<RestaurantOrder?> GetOrderByIdAsync(Guid orderId)
        {
            var orders = await _unitOfWork.Repository<RestaurantOrder>()
                .WhereAsync(o => o.Id == orderId);

            return orders.FirstOrDefault();
        }

        private async System.Threading.Tasks.Task UpdateTableStatusAsync(Guid tableId, TableStatus status, Guid? currentOrderId)
        {
            var tables = await _unitOfWork.Repository<RestaurantTable>()
                .WhereAsync(t => t.Id == tableId);

            var table = tables.FirstOrDefault();
            if (table != null)
            {
                table.Status = status;
                table.CurrentOrderId = currentOrderId;
                table.UpdatedAt = DateTime.UtcNow;

                _unitOfWork.Repository<RestaurantTable>().Update(table);
                await _unitOfWork.SaveChangesAsync();
            }
        }

        private string GenerateOrderNumber()
        {
            return $"ORD-{DateTime.UtcNow:yyyyMMdd}-{Guid.NewGuid().ToString("N")[..8].ToUpper()}";
        }
    }

    // Request/Response DTOs
    public class CreateRestaurantOrderRequest
    {
        public Guid RestaurantId { get; set; }
        public Guid TableId { get; set; }
        public Guid? CustomerId { get; set; }
        public string? CustomerName { get; set; }
        public string? CustomerPhone { get; set; }
        public int NumberOfGuests { get; set; } = 1;
        public OrderType Type { get; set; } = OrderType.DineIn;
        public string? Notes { get; set; }
        public string? SpecialInstructions { get; set; }
        public Guid? AssignedWaiterId { get; set; }
        public Guid CompanyId { get; set; }
        public Guid UserId { get; set; }
        public List<RestaurantOrderMenuItemRequest> MenuItems { get; set; } = new();
    }

    public class RestaurantOrderMenuItemRequest
    {
        public Guid ProductId { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public string? Notes { get; set; }
    }
}
