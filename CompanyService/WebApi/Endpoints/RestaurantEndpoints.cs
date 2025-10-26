using MediatR;
using Microsoft.AspNetCore.Mvc;
using CompanyService.Core.DTOs.Restaurant;
using CompanyService.Core.Feature.Commands.Restaurant;
using CompanyService.Core.Feature.Querys.Restaurant;
using CompanyService.WebApi.Extensions;
using CompanyService.Core.Interfaces;
using CompanyService.Core.Services;

namespace CompanyService.WebApi.Endpoints
{
    public static class RestaurantEndpoints
    {
        public static IEndpointRouteBuilder MapRestaurantEndpoints(this IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("/companies/{companyId:guid}/restaurants")
                .WithTags("Restaurant Management")
                .RequireAuthorization();

            // Restaurant CRUD
            group.MapGet("", GetRestaurants)
                .WithName("GetRestaurants")
                .WithOpenApi()
                .Produces<List<RestaurantDto>>(StatusCodes.Status200OK);

            group.MapPost("", CreateRestaurant)
                .WithName("CreateRestaurant")
                .WithOpenApi()
                .Accepts<CreateRestaurantRequest>("application/json")
                .Produces<RestaurantDto>(StatusCodes.Status201Created);

            group.MapGet("/{id:guid}", GetRestaurantById)
                .WithName("GetRestaurantById")
                .WithOpenApi()
                .Produces<RestaurantDto>(StatusCodes.Status200OK);

            group.MapPut("/{id:guid}", UpdateRestaurant)
                .WithName("UpdateRestaurant")
                .WithOpenApi()
                .Accepts<UpdateRestaurantRequest>("application/json")
                .Produces<RestaurantDto>(StatusCodes.Status200OK);

            group.MapDelete("/{id:guid}", DeleteRestaurant)
                .WithName("DeleteRestaurant")
                .WithOpenApi()
                .Produces(StatusCodes.Status200OK);

            // Restaurant Dashboard
            group.MapGet("/{restaurantId:guid}/dashboard", GetRestaurantDashboard)
                .WithName("GetRestaurantDashboard")
                .WithOpenApi()
                .Produces<RestaurantDashboardDto>(StatusCodes.Status200OK);

            // Restaurant Tables
            group.MapGet("/{restaurantId:guid}/tables", GetRestaurantTables)
                .WithName("GetRestaurantTables")
                .WithOpenApi()
                .Produces<List<RestaurantTableDto>>(StatusCodes.Status200OK);

            group.MapPost("/{restaurantId:guid}/tables", CreateRestaurantTable)
                .WithName("CreateRestaurantTable")
                .WithOpenApi()
                .Accepts<CreateRestaurantTableRequest>("application/json")
                .Produces<RestaurantTableDto>(StatusCodes.Status201Created);

            group.MapGet("/{restaurantId:guid}/tables/{tableId:guid}", GetRestaurantTableById)
                .WithName("GetRestaurantTableById")
                .WithOpenApi()
                .Produces<RestaurantTableDto>(StatusCodes.Status200OK);

            group.MapPut("/{restaurantId:guid}/tables/{tableId:guid}", UpdateRestaurantTable)
                .WithName("UpdateRestaurantTable")
                .WithOpenApi()
                .Accepts<UpdateRestaurantTableRequest>("application/json")
                .Produces<RestaurantTableDto>(StatusCodes.Status200OK);

            group.MapDelete("/{restaurantId:guid}/tables/{tableId:guid}", DeleteRestaurantTable)
                .WithName("DeleteRestaurantTable")
                .WithOpenApi()
                .Produces(StatusCodes.Status200OK);

            group.MapPut("/{restaurantId:guid}/tables/{tableId:guid}/status", ChangeTableStatus)
                .WithName("ChangeTableStatus")
                .WithOpenApi()
                .Accepts<ChangeTableStatusRequest>("application/json")
                .Produces<RestaurantTableDto>(StatusCodes.Status200OK);

            group.MapGet("/{restaurantId:guid}/tables/available", GetAvailableTables)
                .WithName("GetAvailableTables")
                .WithOpenApi()
                .Produces<List<RestaurantTableDto>>(StatusCodes.Status200OK);

            // Restaurant Orders
            group.MapGet("/{restaurantId:guid}/orders", GetRestaurantOrders)
                .WithName("GetRestaurantOrders")
                .WithOpenApi()
                .Produces<List<RestaurantOrderDto>>(StatusCodes.Status200OK);

            group.MapPost("/{restaurantId:guid}/orders", CreateRestaurantOrder)
                .WithName("CreateRestaurantOrder")
                .WithOpenApi()
                .Accepts<CompanyService.Core.DTOs.Restaurant.CreateRestaurantOrderRequest>("application/json")
                .Produces<RestaurantOrderDto>(StatusCodes.Status201Created);

            group.MapGet("/{restaurantId:guid}/orders/{orderId:guid}", GetRestaurantOrderById)
                .WithName("GetRestaurantOrderById")
                .WithOpenApi()
                .Produces<RestaurantOrderDto>(StatusCodes.Status200OK);

            group.MapPut("/{restaurantId:guid}/orders/{orderId:guid}", UpdateRestaurantOrder)
                .WithName("UpdateRestaurantOrder")
                .WithOpenApi()
                .Accepts<UpdateRestaurantOrderRequest>("application/json")
                .Produces<RestaurantOrderDto>(StatusCodes.Status200OK);

            group.MapDelete("/{restaurantId:guid}/orders/{orderId:guid}", DeleteRestaurantOrder)
                .WithName("DeleteRestaurantOrder")
                .WithOpenApi()
                .Produces(StatusCodes.Status200OK);

            group.MapGet("/{restaurantId:guid}/orders/active", GetActiveOrders)
                .WithName("GetActiveOrders")
                .WithOpenApi()
                .Produces<List<RestaurantOrderDto>>(StatusCodes.Status200OK);

            group.MapGet("/{restaurantId:guid}/orders/by-table/{tableId:guid}", GetOrdersByTable)
                .WithName("GetOrdersByTable")
                .WithOpenApi()
                .Produces<List<RestaurantOrderDto>>(StatusCodes.Status200OK);

            // Order Items
            group.MapPost("/{restaurantId:guid}/orders/{orderId:guid}/items", AddOrderItem)
                .WithName("AddOrderItem")
                .WithOpenApi()
                .Accepts<CreateOrderItemRequest>("application/json")
                .Produces<RestaurantOrderItemDto>(StatusCodes.Status201Created);

            group.MapPut("/{restaurantId:guid}/orders/{orderId:guid}/items/{itemId:guid}", UpdateOrderItem)
                .WithName("UpdateOrderItem")
                .WithOpenApi()
                .Accepts<UpdateOrderItemRequest>("application/json")
                .Produces<RestaurantOrderItemDto>(StatusCodes.Status200OK);

            group.MapDelete("/{restaurantId:guid}/orders/{orderId:guid}/items/{itemId:guid}", RemoveOrderItem)
                .WithName("RemoveOrderItem")
                .WithOpenApi()
                .Produces(StatusCodes.Status200OK);

            // Restaurant Payments
            group.MapGet("/{restaurantId:guid}/orders/{orderId:guid}/payments", GetRestaurantPayments)
                .WithName("GetRestaurantPayments")
                .WithOpenApi()
                .Produces<List<RestaurantPaymentDto>>(StatusCodes.Status200OK);

            group.MapPost("/{restaurantId:guid}/orders/{orderId:guid}/payments", CreateRestaurantPayment)
                .WithName("CreateRestaurantPayment")
                .WithOpenApi()
                .Accepts<CreateRestaurantPaymentRequest>("application/json")
                .Produces<RestaurantPaymentDto>(StatusCodes.Status201Created);

            group.MapPost("/{restaurantId:guid}/orders/{orderId:guid}/payments/process", ProcessPayment)
                .WithName("ProcessPayment")
                .WithOpenApi()
                .Accepts<CreateRestaurantPaymentRequest>("application/json")
                .Produces<RestaurantPaymentDto>(StatusCodes.Status200OK);

            // Restaurant Menus
            group.MapGet("/{restaurantId:guid}/menus", GetRestaurantMenus)
                .WithName("GetRestaurantMenus")
                .WithOpenApi()
                .Produces<List<RestaurantMenuDto>>(StatusCodes.Status200OK);

            group.MapPost("/{restaurantId:guid}/menus", CreateRestaurantMenu)
                .WithName("CreateRestaurantMenu")
                .WithOpenApi()
                .Accepts<CreateRestaurantMenuRequest>("application/json")
                .Produces<RestaurantMenuDto>(StatusCodes.Status201Created);

            group.MapGet("/{restaurantId:guid}/menus/{menuId:guid}", GetRestaurantMenuById)
                .WithName("GetRestaurantMenuById")
                .WithOpenApi()
                .Produces<RestaurantMenuDto>(StatusCodes.Status200OK);

            group.MapPut("/{restaurantId:guid}/menus/{menuId:guid}", UpdateRestaurantMenu)
                .WithName("UpdateRestaurantMenu")
                .WithOpenApi()
                .Accepts<UpdateRestaurantMenuRequest>("application/json")
                .Produces<RestaurantMenuDto>(StatusCodes.Status200OK);

            group.MapDelete("/{restaurantId:guid}/menus/{menuId:guid}", DeleteRestaurantMenu)
                .WithName("DeleteRestaurantMenu")
                .WithOpenApi()
                .Produces(StatusCodes.Status200OK);

            // Restaurant Menu Items
            group.MapGet("/{restaurantId:guid}/menus/{menuId:guid}/items", GetRestaurantMenuItems)
                .WithName("GetRestaurantMenuItems")
                .WithOpenApi()
                .Produces<List<RestaurantMenuItemDto>>(StatusCodes.Status200OK);

            group.MapPost("/{restaurantId:guid}/menus/{menuId:guid}/items", CreateRestaurantMenuItem)
                .WithName("CreateRestaurantMenuItem")
                .WithOpenApi()
                .Accepts<CreateRestaurantMenuItemRequest>("application/json")
                .Produces<RestaurantMenuItemDto>(StatusCodes.Status201Created);

            group.MapGet("/{restaurantId:guid}/menus/{menuId:guid}/items/{itemId:guid}", GetRestaurantMenuItemById)
                .WithName("GetRestaurantMenuItemById")
                .WithOpenApi()
                .Produces<RestaurantMenuItemDto>(StatusCodes.Status200OK);

            group.MapPut("/{restaurantId:guid}/menus/{menuId:guid}/items/{itemId:guid}", UpdateRestaurantMenuItem)
                .WithName("UpdateRestaurantMenuItem")
                .WithOpenApi()
                .Accepts<UpdateRestaurantMenuItemRequest>("application/json")
                .Produces<RestaurantMenuItemDto>(StatusCodes.Status200OK);

            group.MapDelete("/{restaurantId:guid}/menus/{menuId:guid}/items/{itemId:guid}", DeleteRestaurantMenuItem)
                .WithName("DeleteRestaurantMenuItem")
                .WithOpenApi()
                .Produces(StatusCodes.Status200OK);

            // Menu Operations
            group.MapGet("/{restaurantId:guid}/menu", GetRestaurantMenu)
                .WithName("GetRestaurantMenu")
                .WithOpenApi()
                .Produces<RestaurantMenuDto>(StatusCodes.Status200OK);

            group.MapGet("/{restaurantId:guid}/menu/with-items", GetRestaurantMenuWithItems)
                .WithName("GetRestaurantMenuWithItems")
                .WithOpenApi()
                .Produces<RestaurantMenuWithItemsDto>(StatusCodes.Status200OK);

            group.MapGet("/{restaurantId:guid}/menu/categories", GetMenuCategories)
                .WithName("GetMenuCategories")
                .WithOpenApi()
                .Produces<List<string>>(StatusCodes.Status200OK);

            group.MapGet("/{restaurantId:guid}/menu/search", SearchMenuItems)
                .WithName("SearchMenuItems")
                .WithOpenApi()
                .Produces<List<RestaurantMenuItemDto>>(StatusCodes.Status200OK);

            // Restaurant Reports
            group.MapGet("/{restaurantId:guid}/reports", GetRestaurantReports)
                .WithName("GetRestaurantReports")
                .WithOpenApi()
                .Produces<RestaurantReportsDto>(StatusCodes.Status200OK);

            group.MapGet("/{restaurantId:guid}/stats", GetRestaurantStats)
                .WithName("GetRestaurantStats")
                .WithOpenApi()
                .Produces<RestaurantStatsDto>(StatusCodes.Status200OK);

            group.MapGet("/{restaurantId:guid}/tables/status", GetTableStatus)
                .WithName("GetTableStatus")
                .WithOpenApi()
                .Produces<List<TableStatusDto>>(StatusCodes.Status200OK);

            return app;
        }

        private static async Task<IResult> GetRestaurants(
            Guid companyId,
            HttpContext httpContext,
            ISender mediator,
            ICacheService cacheService,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string? searchTerm = null,
            [FromQuery] bool? isActive = null)
        {
            var claims = httpContext.ExtractTokenClaims();
            if (claims.UserId is null)
                return Results.Unauthorized();

            try
            {
                var cacheKey = $"restaurants:company:{companyId}:page:{pageNumber}:size:{pageSize}:search:{searchTerm ?? "none"}:active:{isActive?.ToString() ?? "all"}";
                var restaurants = await cacheService.GetAsync<List<RestaurantDto>>(
                    cacheKey,
                    async () => await mediator.Send(new GetRestaurantsQuery
                    {
                        CompanyId = companyId,
                        PageNumber = pageNumber,
                        PageSize = pageSize,
                        SearchTerm = searchTerm,
                        IsActive = isActive
                    }),
                    new CompanyService.Core.Models.Cache.CachePolicy { Expiry = TimeSpan.FromMinutes(10) });

                return Results.Ok(restaurants);
            }
            catch (Exception ex)
            {
                return Results.BadRequest(new { error = ex.Message });
            }
        }

        private static async Task<IResult> CreateRestaurant(
            Guid companyId,
            CreateRestaurantRequest request,
            HttpContext httpContext,
            ISender mediator)
        {
            var claims = httpContext.ExtractTokenClaims();
            if (claims.UserId is null)
                return Results.Unauthorized();

            try
            {
                var command = new CreateRestaurantCommand
                {
                    Name = request.Name,
                    Description = request.Description,
                    Address = request.Address,
                    City = request.City,
                    Phone = request.Phone,
                    Email = request.Email,
                    RUC = request.RUC,
                    CompanyId = companyId,
                    UserId = Guid.Parse(claims.UserId)
                };

                var restaurant = await mediator.Send(command);
                return Results.Created($"/companies/{companyId}/restaurants/{restaurant.Id}", restaurant);
            }
            catch (Exception ex)
            {
                return Results.BadRequest(new { error = ex.Message });
            }
        }

        private static async Task<IResult> GetRestaurantById(
            Guid companyId,
            Guid id,
            HttpContext httpContext,
            ISender mediator,
            ICacheService cacheService)
        {
            var claims = httpContext.ExtractTokenClaims();
            if (claims.UserId is null)
                return Results.Unauthorized();

            try
            {
                var cacheKey = $"restaurant:company:{companyId}:id:{id}";
                var restaurant = await cacheService.GetAsync<RestaurantDto?>(
                    cacheKey,
                    async () => await mediator.Send(new GetRestaurantByIdQuery
                    {
                        Id = id,
                        CompanyId = companyId
                    }),
                    new CompanyService.Core.Models.Cache.CachePolicy { Expiry = TimeSpan.FromMinutes(15) });

                if (restaurant == null)
                    return Results.NotFound();

                return Results.Ok(restaurant);
            }
            catch (Exception ex)
            {
                return Results.BadRequest(new { error = ex.Message });
            }
        }

        private static async Task<IResult> UpdateRestaurant(
            Guid companyId,
            Guid id,
            UpdateRestaurantRequest request,
            HttpContext httpContext,
            ISender mediator)
        {
            var claims = httpContext.ExtractTokenClaims();
            if (claims.UserId is null)
                return Results.Unauthorized();

            try
            {
                var command = new UpdateRestaurantCommand
                {
                    Id = id,
                    Name = request.Name,
                    Description = request.Description,
                    Address = request.Address,
                    City = request.City,
                    Phone = request.Phone,
                    Email = request.Email,
                    RUC = request.RUC,
                    IsActive = request.IsActive,
                    CompanyId = companyId,
                    UserId = Guid.Parse(claims.UserId)
                };

                var restaurant = await mediator.Send(command);
                return Results.Ok(restaurant);
            }
            catch (Exception ex)
            {
                return Results.BadRequest(new { error = ex.Message });
            }
        }

        private static async Task<IResult> DeleteRestaurant(
            Guid companyId,
            Guid id,
            HttpContext httpContext,
            ISender mediator)
        {
            var claims = httpContext.ExtractTokenClaims();
            if (claims.UserId is null)
                return Results.Unauthorized();

            try
            {
                var command = new DeleteRestaurantCommand
                {
                    Id = id,
                    CompanyId = companyId,
                    UserId = Guid.Parse(claims.UserId)
                };

                await mediator.Send(command);
                return Results.Ok();
            }
            catch (Exception ex)
            {
                return Results.BadRequest(new { error = ex.Message });
            }
        }

        private static async Task<IResult> GetRestaurantDashboard(
            Guid companyId,
            Guid restaurantId,
            HttpContext httpContext,
            ISender mediator,
            ICacheService cacheService)
        {
            var claims = httpContext.ExtractTokenClaims();
            if (claims.UserId is null)
                return Results.Unauthorized();

            try
            {
                var cacheKey = $"restaurant-dashboard:company:{companyId}:restaurant:{restaurantId}";
                var dashboard = await cacheService.GetAsync<RestaurantDashboardDto>(
                    cacheKey,
                    async () => await mediator.Send(new GetRestaurantDashboardQuery
                    {
                        RestaurantId = restaurantId,
                        CompanyId = companyId
                    }),
                    new CompanyService.Core.Models.Cache.CachePolicy { Expiry = TimeSpan.FromMinutes(5) });

                return Results.Ok(dashboard);
            }
            catch (Exception ex)
            {
                return Results.BadRequest(new { error = ex.Message });
            }
        }

        private static async Task<IResult> GetRestaurantTables(
            Guid companyId,
            Guid restaurantId,
            HttpContext httpContext,
            ISender mediator,
            ICacheService cacheService,
            [FromQuery] int? status = null,
            [FromQuery] bool? isActive = null)
        {
            var claims = httpContext.ExtractTokenClaims();
            if (claims.UserId is null)
                return Results.Unauthorized();

            try
            {
                var cacheKey = $"restaurant-tables:company:{companyId}:restaurant:{restaurantId}:status:{status?.ToString() ?? "all"}:active:{isActive?.ToString() ?? "all"}";
                var tables = await cacheService.GetAsync<List<RestaurantTableDto>>(
                    cacheKey,
                    async () => await mediator.Send(new GetRestaurantTablesQuery
                    {
                        RestaurantId = restaurantId,
                        CompanyId = companyId,
                        Status = status,
                        IsActive = isActive
                    }),
                    new CompanyService.Core.Models.Cache.CachePolicy { Expiry = TimeSpan.FromMinutes(10) });

                return Results.Ok(tables);
            }
            catch (Exception ex)
            {
                return Results.BadRequest(new { error = ex.Message });
            }
        }

        private static async Task<IResult> CreateRestaurantTable(
            Guid companyId,
            Guid restaurantId,
            CreateRestaurantTableRequest request,
            HttpContext httpContext,
            ISender mediator)
        {
            var claims = httpContext.ExtractTokenClaims();
            if (claims.UserId is null)
                return Results.Unauthorized();

            try
            {
                var command = new CreateRestaurantTableCommand
                {
                    TableNumber = request.TableNumber,
                    Name = request.Name,
                    Capacity = request.Capacity,
                    Location = request.Location,
                    Description = request.Description,
                    RestaurantId = restaurantId,
                    CompanyId = companyId,
                    UserId = Guid.Parse(claims.UserId)
                };

                var table = await mediator.Send(command);
                return Results.Created($"/companies/{companyId}/restaurants/{restaurantId}/tables/{table.Id}", table);
            }
            catch (Exception ex)
            {
                return Results.BadRequest(new { error = ex.Message });
            }
        }

        private static async Task<IResult> GetRestaurantTableById(
            Guid companyId,
            Guid restaurantId,
            Guid tableId,
            HttpContext httpContext,
            ISender mediator,
            ICacheService cacheService)
        {
            var claims = httpContext.ExtractTokenClaims();
            if (claims.UserId is null)
                return Results.Unauthorized();

            try
            {
                var cacheKey = $"restaurant-table:company:{companyId}:restaurant:{restaurantId}:table:{tableId}";
                var table = await cacheService.GetAsync<RestaurantTableDto?>(
                    cacheKey,
                    async () => await mediator.Send(new GetRestaurantTableByIdQuery
                    {
                        Id = tableId,
                        RestaurantId = restaurantId,
                        CompanyId = companyId
                    }),
                    new CompanyService.Core.Models.Cache.CachePolicy { Expiry = TimeSpan.FromMinutes(15) });

                if (table == null)
                    return Results.NotFound();

                return Results.Ok(table);
            }
            catch (Exception ex)
            {
                return Results.BadRequest(new { error = ex.Message });
            }
        }

        private static async Task<IResult> UpdateRestaurantTable(
            Guid companyId,
            Guid restaurantId,
            Guid tableId,
            UpdateRestaurantTableRequest request,
            HttpContext httpContext,
            ISender mediator)
        {
            var claims = httpContext.ExtractTokenClaims();
            if (claims.UserId is null)
                return Results.Unauthorized();

            try
            {
                var command = new UpdateRestaurantTableCommand
                {
                    Id = tableId,
                    TableNumber = request.TableNumber,
                    Name = request.Name,
                    Capacity = request.Capacity,
                    Location = request.Location,
                    Description = request.Description,
                    IsActive = request.IsActive,
                    RestaurantId = restaurantId,
                    CompanyId = companyId,
                    UserId = Guid.Parse(claims.UserId)
                };

                var table = await mediator.Send(command);
                return Results.Ok(table);
            }
            catch (Exception ex)
            {
                return Results.BadRequest(new { error = ex.Message });
            }
        }

        private static async Task<IResult> DeleteRestaurantTable(
            Guid companyId,
            Guid restaurantId,
            Guid tableId,
            HttpContext httpContext,
            ISender mediator)
        {
            var claims = httpContext.ExtractTokenClaims();
            if (claims.UserId is null)
                return Results.Unauthorized();

            try
            {
                var command = new DeleteRestaurantTableCommand
                {
                    Id = tableId,
                    RestaurantId = restaurantId,
                    CompanyId = companyId,
                    UserId = Guid.Parse(claims.UserId)
                };

                await mediator.Send(command);
                return Results.Ok();
            }
            catch (Exception ex)
            {
                return Results.BadRequest(new { error = ex.Message });
            }
        }

        private static async Task<IResult> ChangeTableStatus(
            Guid companyId,
            Guid restaurantId,
            Guid tableId,
            ChangeTableStatusRequest request,
            HttpContext httpContext,
            ISender mediator)
        {
            var claims = httpContext.ExtractTokenClaims();
            if (claims.UserId is null)
                return Results.Unauthorized();

            try
            {
                var command = new ChangeTableStatusCommand
                {
                    Id = tableId,
                    Status = request.Status,
                    Notes = request.Notes,
                    RestaurantId = restaurantId,
                    CompanyId = companyId,
                    UserId = Guid.Parse(claims.UserId)
                };

                var table = await mediator.Send(command);
                return Results.Ok(table);
            }
            catch (Exception ex)
            {
                return Results.BadRequest(new { error = ex.Message });
            }
        }

        private static async Task<IResult> GetAvailableTables(
            Guid companyId,
            Guid restaurantId,
            HttpContext httpContext,
            ISender mediator,
            ICacheService cacheService,
            [FromQuery] int? minCapacity = null)
        {
            var claims = httpContext.ExtractTokenClaims();
            if (claims.UserId is null)
                return Results.Unauthorized();

            try
            {
                var cacheKey = $"available-tables:company:{companyId}:restaurant:{restaurantId}:minCapacity:{minCapacity?.ToString() ?? "any"}";
                var tables = await cacheService.GetAsync<List<RestaurantTableDto>>(
                    cacheKey,
                    async () => await mediator.Send(new GetAvailableTablesQuery
                    {
                        RestaurantId = restaurantId,
                        CompanyId = companyId,
                        MinCapacity = minCapacity
                    }),
                    new CompanyService.Core.Models.Cache.CachePolicy { Expiry = TimeSpan.FromMinutes(5) });

                return Results.Ok(tables);
            }
            catch (Exception ex)
            {
                return Results.BadRequest(new { error = ex.Message });
            }
        }

        // Restaurant Orders Endpoints
        private static async Task<IResult> GetRestaurantOrders(
            Guid companyId,
            Guid restaurantId,
            HttpContext httpContext,
            ISender mediator,
            ICacheService cacheService,
            [FromQuery] int? status = null,
            [FromQuery] int? type = null,
            [FromQuery] Guid? tableId = null,
            [FromQuery] int pageSize = 50)
        {
            var claims = httpContext.ExtractTokenClaims();
            if (claims.UserId is null)
                return Results.Unauthorized();

            try
            {
                var cacheKey = $"restaurant-orders:company:{companyId}:restaurant:{restaurantId}:status:{status?.ToString() ?? "all"}:type:{type?.ToString() ?? "all"}:table:{tableId?.ToString() ?? "all"}";
                var orders = await cacheService.GetAsync<List<RestaurantOrderDto>>(
                    cacheKey,
                    async () => await mediator.Send(new GetRestaurantOrdersQuery
                    {
                        RestaurantId = restaurantId,
                        CompanyId = companyId,
                        Status = status,
                        Type = type,
                        TableId = tableId,
                        PageSize = pageSize
                    }),
                    new CompanyService.Core.Models.Cache.CachePolicy { Expiry = TimeSpan.FromMinutes(5) });

                return Results.Ok(orders);
            }
            catch (Exception ex)
            {
                return Results.BadRequest(new { error = ex.Message });
            }
        }

        private static async Task<IResult> CreateRestaurantOrder(
            Guid companyId,
            Guid restaurantId,
            CompanyService.Core.DTOs.Restaurant.CreateRestaurantOrderRequest request,
            HttpContext httpContext,
            ISender mediator)
        {
            var claims = httpContext.ExtractTokenClaims();
            if (claims.UserId is null)
                return Results.Unauthorized();

            try
            {
                var command = new CreateRestaurantOrderCommand
                {
                    TableId = request.TableId,
                    Type = request.Type,
                    CustomerName = request.CustomerName,
                    CustomerPhone = request.CustomerPhone,
                    NumberOfGuests = request.NumberOfGuests,
                    Notes = request.Notes,
                    SpecialInstructions = request.SpecialInstructions,
                    OrderItems = request.OrderItems.Select(oi => new CreateOrderItemCommand
                    {
                        MenuItemId = oi.MenuItemId,
                        Quantity = oi.Quantity,
                        SpecialInstructions = oi.SpecialInstructions
                    }).ToList(),
                    RestaurantId = restaurantId,
                    CompanyId = companyId,
                    UserId = Guid.Parse(claims.UserId)
                };

                var order = await mediator.Send(command);
                return Results.Created($"/companies/{companyId}/restaurants/{restaurantId}/orders/{order.Id}", order);
            }
            catch (Exception ex)
            {
                return Results.BadRequest(new { error = ex.Message });
            }
        }

        private static async Task<IResult> GetRestaurantOrderById(
            Guid companyId,
            Guid restaurantId,
            Guid orderId,
            HttpContext httpContext,
            ISender mediator,
            ICacheService cacheService)
        {
            var claims = httpContext.ExtractTokenClaims();
            if (claims.UserId is null)
                return Results.Unauthorized();

            try
            {
                var cacheKey = $"restaurant-order:company:{companyId}:restaurant:{restaurantId}:order:{orderId}";
                var order = await cacheService.GetAsync<RestaurantOrderDto?>(
                    cacheKey,
                    async () => await mediator.Send(new GetRestaurantOrderByIdQuery
                    {
                        Id = orderId,
                        RestaurantId = restaurantId,
                        CompanyId = companyId
                    }),
                    new CompanyService.Core.Models.Cache.CachePolicy { Expiry = TimeSpan.FromMinutes(10) });

                if (order == null)
                    return Results.NotFound();

                return Results.Ok(order);
            }
            catch (Exception ex)
            {
                return Results.BadRequest(new { error = ex.Message });
            }
        }

        private static async Task<IResult> UpdateRestaurantOrder(
            Guid companyId,
            Guid restaurantId,
            Guid orderId,
            UpdateRestaurantOrderRequest request,
            HttpContext httpContext,
            ISender mediator)
        {
            var claims = httpContext.ExtractTokenClaims();
            if (claims.UserId is null)
                return Results.Unauthorized();

            try
            {
                var command = new UpdateRestaurantOrderCommand
                {
                    Id = orderId,
                    Status = request.Status,
                    CustomerName = request.CustomerName,
                    CustomerPhone = request.CustomerPhone,
                    NumberOfGuests = request.NumberOfGuests,
                    Notes = request.Notes,
                    SpecialInstructions = request.SpecialInstructions,
                    AssignedWaiterId = request.AssignedWaiterId,
                    RestaurantId = restaurantId,
                    CompanyId = companyId,
                    UserId = Guid.Parse(claims.UserId)
                };

                var order = await mediator.Send(command);
                return Results.Ok(order);
            }
            catch (Exception ex)
            {
                return Results.BadRequest(new { error = ex.Message });
            }
        }

        private static async Task<IResult> DeleteRestaurantOrder(
            Guid companyId,
            Guid restaurantId,
            Guid orderId,
            HttpContext httpContext,
            ISender mediator)
        {
            var claims = httpContext.ExtractTokenClaims();
            if (claims.UserId is null)
                return Results.Unauthorized();

            try
            {
                var command = new DeleteRestaurantOrderCommand
                {
                    Id = orderId,
                    RestaurantId = restaurantId,
                    CompanyId = companyId,
                    UserId = Guid.Parse(claims.UserId)
                };

                await mediator.Send(command);
                return Results.Ok();
            }
            catch (Exception ex)
            {
                return Results.BadRequest(new { error = ex.Message });
            }
        }

        private static async Task<IResult> GetActiveOrders(
            Guid companyId,
            Guid restaurantId,
            HttpContext httpContext,
            ISender mediator,
            ICacheService cacheService)
        {
            var claims = httpContext.ExtractTokenClaims();
            if (claims.UserId is null)
                return Results.Unauthorized();

            try
            {
                var cacheKey = $"active-orders:company:{companyId}:restaurant:{restaurantId}";
                var orders = await cacheService.GetAsync<List<RestaurantOrderDto>>(
                    cacheKey,
                    async () => await mediator.Send(new GetActiveOrdersQuery
                    {
                        RestaurantId = restaurantId,
                        CompanyId = companyId
                    }),
                    new CompanyService.Core.Models.Cache.CachePolicy { Expiry = TimeSpan.FromMinutes(2) });

                return Results.Ok(orders);
            }
            catch (Exception ex)
            {
                return Results.BadRequest(new { error = ex.Message });
            }
        }

        private static async Task<IResult> GetOrdersByTable(
            Guid companyId,
            Guid restaurantId,
            Guid tableId,
            HttpContext httpContext,
            ISender mediator,
            ICacheService cacheService)
        {
            var claims = httpContext.ExtractTokenClaims();
            if (claims.UserId is null)
                return Results.Unauthorized();

            try
            {
                var cacheKey = $"orders-by-table:company:{companyId}:restaurant:{restaurantId}:table:{tableId}";
                var orders = await cacheService.GetAsync<List<RestaurantOrderDto>>(
                    cacheKey,
                    async () => await mediator.Send(new GetOrdersByTableQuery
                    {
                        TableId = tableId,
                        RestaurantId = restaurantId,
                        CompanyId = companyId
                    }),
                    new CompanyService.Core.Models.Cache.CachePolicy { Expiry = TimeSpan.FromMinutes(5) });

                return Results.Ok(orders);
            }
            catch (Exception ex)
            {
                return Results.BadRequest(new { error = ex.Message });
            }
        }

        // Order Items Endpoints
        private static async Task<IResult> AddOrderItem(
            Guid companyId,
            Guid restaurantId,
            Guid orderId,
            CreateOrderItemRequest request,
            HttpContext httpContext,
            ISender mediator)
        {
            var claims = httpContext.ExtractTokenClaims();
            if (claims.UserId is null)
                return Results.Unauthorized();

            try
            {
                var command = new AddOrderItemCommand
                {
                    OrderId = orderId,
                    MenuItemId = request.MenuItemId,
                    Quantity = request.Quantity,
                    SpecialInstructions = request.SpecialInstructions,
                    RestaurantId = restaurantId,
                    CompanyId = companyId,
                    UserId = Guid.Parse(claims.UserId)
                };

                var orderItem = await mediator.Send(command);
                return Results.Created($"/companies/{companyId}/restaurants/{restaurantId}/orders/{orderId}/items/{orderItem.Id}", orderItem);
            }
            catch (Exception ex)
            {
                return Results.BadRequest(new { error = ex.Message });
            }
        }

        private static async Task<IResult> UpdateOrderItem(
            Guid companyId,
            Guid restaurantId,
            Guid orderId,
            Guid itemId,
            UpdateOrderItemRequest request,
            HttpContext httpContext,
            ISender mediator)
        {
            var claims = httpContext.ExtractTokenClaims();
            if (claims.UserId is null)
                return Results.Unauthorized();

            try
            {
                var command = new UpdateOrderItemCommand
                {
                    Id = itemId,
                    Quantity = request.Quantity,
                    SpecialInstructions = request.SpecialInstructions,
                    Status = request.Status,
                    OrderId = orderId,
                    RestaurantId = restaurantId,
                    CompanyId = companyId,
                    UserId = Guid.Parse(claims.UserId)
                };

                var orderItem = await mediator.Send(command);
                return Results.Ok(orderItem);
            }
            catch (Exception ex)
            {
                return Results.BadRequest(new { error = ex.Message });
            }
        }

        private static async Task<IResult> RemoveOrderItem(
            Guid companyId,
            Guid restaurantId,
            Guid orderId,
            Guid itemId,
            HttpContext httpContext,
            ISender mediator)
        {
            var claims = httpContext.ExtractTokenClaims();
            if (claims.UserId is null)
                return Results.Unauthorized();

            try
            {
                var command = new RemoveOrderItemCommand
                {
                    Id = itemId,
                    OrderId = orderId,
                    RestaurantId = restaurantId,
                    CompanyId = companyId,
                    UserId = Guid.Parse(claims.UserId)
                };

                await mediator.Send(command);
                return Results.Ok();
            }
            catch (Exception ex)
            {
                return Results.BadRequest(new { error = ex.Message });
            }
        }

        // Restaurant Payments Endpoints
        private static async Task<IResult> GetRestaurantPayments(
            Guid companyId,
            Guid restaurantId,
            Guid orderId,
            HttpContext httpContext,
            ISender mediator,
            ICacheService cacheService)
        {
            var claims = httpContext.ExtractTokenClaims();
            if (claims.UserId is null)
                return Results.Unauthorized();

            try
            {
                var cacheKey = $"restaurant-payments:company:{companyId}:restaurant:{restaurantId}:order:{orderId}";
                var payments = await cacheService.GetAsync<List<RestaurantPaymentDto>>(
                    cacheKey,
                    async () => await mediator.Send(new GetRestaurantPaymentsQuery
                    {
                        OrderId = orderId,
                        RestaurantId = restaurantId,
                        CompanyId = companyId
                    }),
                    new CompanyService.Core.Models.Cache.CachePolicy { Expiry = TimeSpan.FromMinutes(10) });

                return Results.Ok(payments);
            }
            catch (Exception ex)
            {
                return Results.BadRequest(new { error = ex.Message });
            }
        }

        private static async Task<IResult> CreateRestaurantPayment(
            Guid companyId,
            Guid restaurantId,
            Guid orderId,
            CreateRestaurantPaymentRequest request,
            HttpContext httpContext,
            ISender mediator)
        {
            var claims = httpContext.ExtractTokenClaims();
            if (claims.UserId is null)
                return Results.Unauthorized();

            try
            {
                var command = new CreateRestaurantPaymentCommand
                {
                    OrderId = orderId,
                    Amount = request.Amount,
                    Method = request.Method,
                    TransactionId = request.TransactionId,
                    Notes = request.Notes,
                    RestaurantId = restaurantId,
                    CompanyId = companyId,
                    UserId = Guid.Parse(claims.UserId)
                };

                var payment = await mediator.Send(command);
                return Results.Created($"/companies/{companyId}/restaurants/{restaurantId}/orders/{orderId}/payments/{payment.Id}", payment);
            }
            catch (Exception ex)
            {
                return Results.BadRequest(new { error = ex.Message });
            }
        }

        private static async Task<IResult> ProcessPayment(
            Guid companyId,
            Guid restaurantId,
            Guid orderId,
            CreateRestaurantPaymentRequest request,
            HttpContext httpContext,
            ISender mediator)
        {
            var claims = httpContext.ExtractTokenClaims();
            if (claims.UserId is null)
                return Results.Unauthorized();

            try
            {
                var command = new ProcessPaymentCommand
                {
                    OrderId = orderId,
                    Amount = request.Amount,
                    Method = request.Method,
                    TransactionId = request.TransactionId,
                    Notes = request.Notes,
                    RestaurantId = restaurantId,
                    CompanyId = companyId,
                    UserId = Guid.Parse(claims.UserId)
                };

                var payment = await mediator.Send(command);
                return Results.Ok(payment);
            }
            catch (Exception ex)
            {
                return Results.BadRequest(new { error = ex.Message });
            }
        }

        // Restaurant Menus Endpoints
        private static async Task<IResult> GetRestaurantMenus(
            Guid companyId,
            Guid restaurantId,
            HttpContext httpContext,
            ISender mediator,
            ICacheService cacheService,
            [FromQuery] bool? isActive = null)
        {
            var claims = httpContext.ExtractTokenClaims();
            if (claims.UserId is null)
                return Results.Unauthorized();

            try
            {
                var cacheKey = $"restaurant-menus:company:{companyId}:restaurant:{restaurantId}:active:{isActive?.ToString() ?? "all"}";
                var menus = await cacheService.GetAsync<List<RestaurantMenuDto>>(
                    cacheKey,
                    async () => await mediator.Send(new GetRestaurantMenusQuery
                    {
                        RestaurantId = restaurantId,
                        CompanyId = companyId,
                        IsActive = isActive
                    }),
                    new CompanyService.Core.Models.Cache.CachePolicy { Expiry = TimeSpan.FromMinutes(15) });

                return Results.Ok(menus);
            }
            catch (Exception ex)
            {
                return Results.BadRequest(new { error = ex.Message });
            }
        }

        private static async Task<IResult> CreateRestaurantMenu(
            Guid companyId,
            Guid restaurantId,
            CreateRestaurantMenuRequest request,
            HttpContext httpContext,
            ISender mediator)
        {
            var claims = httpContext.ExtractTokenClaims();
            if (claims.UserId is null)
                return Results.Unauthorized();

            try
            {
                var command = new CreateRestaurantMenuCommand
                {
                    Name = request.Name,
                    Description = request.Description,
                    RestaurantId = restaurantId,
                    CompanyId = companyId,
                    UserId = Guid.Parse(claims.UserId)
                };

                var menu = await mediator.Send(command);
                return Results.Created($"/companies/{companyId}/restaurants/{restaurantId}/menus/{menu.Id}", menu);
            }
            catch (Exception ex)
            {
                return Results.BadRequest(new { error = ex.Message });
            }
        }

        private static async Task<IResult> GetRestaurantMenuById(
            Guid companyId,
            Guid restaurantId,
            Guid menuId,
            HttpContext httpContext,
            ISender mediator,
            ICacheService cacheService)
        {
            var claims = httpContext.ExtractTokenClaims();
            if (claims.UserId is null)
                return Results.Unauthorized();

            try
            {
                var cacheKey = $"restaurant-menu:company:{companyId}:restaurant:{restaurantId}:menu:{menuId}";
                var menu = await cacheService.GetAsync<RestaurantMenuDto?>(
                    cacheKey,
                    async () => await mediator.Send(new GetRestaurantMenuByIdQuery
                    {
                        Id = menuId,
                        RestaurantId = restaurantId,
                        CompanyId = companyId
                    }),
                    new CompanyService.Core.Models.Cache.CachePolicy { Expiry = TimeSpan.FromMinutes(15) });

                if (menu == null)
                    return Results.NotFound();

                return Results.Ok(menu);
            }
            catch (Exception ex)
            {
                return Results.BadRequest(new { error = ex.Message });
            }
        }

        private static async Task<IResult> UpdateRestaurantMenu(
            Guid companyId,
            Guid restaurantId,
            Guid menuId,
            UpdateRestaurantMenuRequest request,
            HttpContext httpContext,
            ISender mediator)
        {
            var claims = httpContext.ExtractTokenClaims();
            if (claims.UserId is null)
                return Results.Unauthorized();

            try
            {
                var command = new UpdateRestaurantMenuCommand
                {
                    Id = menuId,
                    Name = request.Name,
                    Description = request.Description,
                    IsActive = request.IsActive,
                    RestaurantId = restaurantId,
                    CompanyId = companyId,
                    UserId = Guid.Parse(claims.UserId)
                };

                var menu = await mediator.Send(command);
                return Results.Ok(menu);
            }
            catch (Exception ex)
            {
                return Results.BadRequest(new { error = ex.Message });
            }
        }

        private static async Task<IResult> DeleteRestaurantMenu(
            Guid companyId,
            Guid restaurantId,
            Guid menuId,
            HttpContext httpContext,
            ISender mediator)
        {
            var claims = httpContext.ExtractTokenClaims();
            if (claims.UserId is null)
                return Results.Unauthorized();

            try
            {
                var command = new DeleteRestaurantMenuCommand
                {
                    Id = menuId,
                    RestaurantId = restaurantId,
                    CompanyId = companyId,
                    UserId = Guid.Parse(claims.UserId)
                };

                await mediator.Send(command);
                return Results.Ok();
            }
            catch (Exception ex)
            {
                return Results.BadRequest(new { error = ex.Message });
            }
        }

        // Restaurant Menu Items Endpoints
        private static async Task<IResult> GetRestaurantMenuItems(
            Guid companyId,
            Guid restaurantId,
            Guid menuId,
            HttpContext httpContext,
            ISender mediator,
            ICacheService cacheService,
            [FromQuery] string? category = null,
            [FromQuery] bool? isAvailable = null,
            [FromQuery] bool? isVegetarian = null,
            [FromQuery] bool? isVegan = null,
            [FromQuery] bool? isGlutenFree = null)
        {
            var claims = httpContext.ExtractTokenClaims();
            if (claims.UserId is null)
                return Results.Unauthorized();

            try
            {
                var cacheKey = $"restaurant-menu-items:company:{companyId}:restaurant:{restaurantId}:menu:{menuId}:category:{category ?? "all"}:available:{isAvailable?.ToString() ?? "all"}:vegetarian:{isVegetarian?.ToString() ?? "all"}:vegan:{isVegan?.ToString() ?? "all"}:glutenfree:{isGlutenFree?.ToString() ?? "all"}";
                var items = await cacheService.GetAsync<List<RestaurantMenuItemDto>>(
                    cacheKey,
                    async () => await mediator.Send(new GetRestaurantMenuItemsQuery
                    {
                        MenuId = menuId,
                        RestaurantId = restaurantId,
                        CompanyId = companyId,
                        Category = category,
                        IsAvailable = isAvailable,
                        IsVegetarian = isVegetarian,
                        IsVegan = isVegan,
                        IsGlutenFree = isGlutenFree
                    }),
                    new CompanyService.Core.Models.Cache.CachePolicy { Expiry = TimeSpan.FromMinutes(10) });

                return Results.Ok(items);
            }
            catch (Exception ex)
            {
                return Results.BadRequest(new { error = ex.Message });
            }
        }

        private static async Task<IResult> CreateRestaurantMenuItem(
            Guid companyId,
            Guid restaurantId,
            Guid menuId,
            CreateRestaurantMenuItemRequest request,
            HttpContext httpContext,
            ISender mediator)
        {
            var claims = httpContext.ExtractTokenClaims();
            if (claims.UserId is null)
                return Results.Unauthorized();

            try
            {
                var command = new CreateRestaurantMenuItemCommand
                {
                    Name = request.Name,
                    Description = request.Description,
                    Price = request.Price,
                    Category = request.Category,
                    ImageUrl = request.ImageUrl,
                    PreparationTime = request.PreparationTime ?? 0,
                    Allergens = request.Allergens,
                    IsVegetarian = request.IsVegetarian,
                    IsVegan = request.IsVegan,
                    IsGlutenFree = request.IsGlutenFree,
                    MenuId = menuId,
                    RestaurantId = restaurantId,
                    CompanyId = companyId,
                    UserId = Guid.Parse(claims.UserId)
                };

                var menuItem = await mediator.Send(command);
                return Results.Created($"/companies/{companyId}/restaurants/{restaurantId}/menus/{menuId}/items/{menuItem.Id}", menuItem);
            }
            catch (Exception ex)
            {
                return Results.BadRequest(new { error = ex.Message });
            }
        }

        private static async Task<IResult> GetRestaurantMenuItemById(
            Guid companyId,
            Guid restaurantId,
            Guid menuId,
            Guid itemId,
            HttpContext httpContext,
            ISender mediator,
            ICacheService cacheService)
        {
            var claims = httpContext.ExtractTokenClaims();
            if (claims.UserId is null)
                return Results.Unauthorized();

            try
            {
                var cacheKey = $"restaurant-menu-item:company:{companyId}:restaurant:{restaurantId}:menu:{menuId}:item:{itemId}";
                var menuItem = await cacheService.GetAsync<RestaurantMenuItemDto?>(
                    cacheKey,
                    async () => await mediator.Send(new GetRestaurantMenuItemByIdQuery
                    {
                        Id = itemId,
                        MenuId = menuId,
                        RestaurantId = restaurantId,
                        CompanyId = companyId
                    }),
                    new CompanyService.Core.Models.Cache.CachePolicy { Expiry = TimeSpan.FromMinutes(15) });

                if (menuItem == null)
                    return Results.NotFound();

                return Results.Ok(menuItem);
            }
            catch (Exception ex)
            {
                return Results.BadRequest(new { error = ex.Message });
            }
        }

        private static async Task<IResult> UpdateRestaurantMenuItem(
            Guid companyId,
            Guid restaurantId,
            Guid menuId,
            Guid itemId,
            UpdateRestaurantMenuItemRequest request,
            HttpContext httpContext,
            ISender mediator)
        {
            var claims = httpContext.ExtractTokenClaims();
            if (claims.UserId is null)
                return Results.Unauthorized();

            try
            {
                var command = new UpdateRestaurantMenuItemCommand
                {
                    Id = itemId,
                    Name = request.Name,
                    Description = request.Description,
                    Price = request.Price,
                    Category = request.Category,
                    ImageUrl = request.ImageUrl,
                    PreparationTime = request.PreparationTime ?? 0,
                    Allergens = request.Allergens,
                    IsVegetarian = request.IsVegetarian,
                    IsVegan = request.IsVegan,
                    IsGlutenFree = request.IsGlutenFree,
                    IsAvailable = request.IsAvailable,
                    MenuId = menuId,
                    RestaurantId = restaurantId,
                    CompanyId = companyId,
                    UserId = Guid.Parse(claims.UserId)
                };

                var menuItem = await mediator.Send(command);
                return Results.Ok(menuItem);
            }
            catch (Exception ex)
            {
                return Results.BadRequest(new { error = ex.Message });
            }
        }

        private static async Task<IResult> DeleteRestaurantMenuItem(
            Guid companyId,
            Guid restaurantId,
            Guid menuId,
            Guid itemId,
            HttpContext httpContext,
            ISender mediator)
        {
            var claims = httpContext.ExtractTokenClaims();
            if (claims.UserId is null)
                return Results.Unauthorized();

            try
            {
                var command = new DeleteRestaurantMenuItemCommand
                {
                    Id = itemId,
                    MenuId = menuId,
                    RestaurantId = restaurantId,
                    CompanyId = companyId,
                    UserId = Guid.Parse(claims.UserId)
                };

                await mediator.Send(command);
                return Results.Ok();
            }
            catch (Exception ex)
            {
                return Results.BadRequest(new { error = ex.Message });
            }
        }

        // Menu Operations Endpoints
        private static async Task<IResult> GetRestaurantMenu(
            Guid companyId,
            Guid restaurantId,
            HttpContext httpContext,
            ISender mediator,
            ICacheService cacheService)
        {
            var claims = httpContext.ExtractTokenClaims();
            if (claims.UserId is null)
                return Results.Unauthorized();

            try
            {
                var cacheKey = $"restaurant-menu:company:{companyId}:restaurant:{restaurantId}";
                var menu = await cacheService.GetAsync<RestaurantMenuDto?>(
                    cacheKey,
                    async () => await mediator.Send(new GetRestaurantMenuQuery
                    {
                        RestaurantId = restaurantId,
                        CompanyId = companyId
                    }),
                    new CompanyService.Core.Models.Cache.CachePolicy { Expiry = TimeSpan.FromMinutes(15) });

                if (menu == null)
                    return Results.NotFound();

                return Results.Ok(menu);
            }
            catch (Exception ex)
            {
                return Results.BadRequest(new { error = ex.Message });
            }
        }

        private static async Task<IResult> GetRestaurantMenuWithItems(
            Guid companyId,
            Guid restaurantId,
            HttpContext httpContext,
            ISender mediator,
            ICacheService cacheService,
            [FromQuery] string? category = null,
            [FromQuery] bool? isAvailable = null,
            [FromQuery] bool? isVegetarian = null,
            [FromQuery] bool? isVegan = null,
            [FromQuery] bool? isGlutenFree = null)
        {
            var claims = httpContext.ExtractTokenClaims();
            if (claims.UserId is null)
                return Results.Unauthorized();

            try
            {
                var cacheKey = $"restaurant-menu-with-items:company:{companyId}:restaurant:{restaurantId}:category:{category ?? "all"}:available:{isAvailable?.ToString() ?? "all"}:vegetarian:{isVegetarian?.ToString() ?? "all"}:vegan:{isVegan?.ToString() ?? "all"}:glutenfree:{isGlutenFree?.ToString() ?? "all"}";
                var menu = await cacheService.GetAsync<RestaurantMenuWithItemsDto?>(
                    cacheKey,
                    async () => await mediator.Send(new GetRestaurantMenuWithItemsQuery
                    {
                        RestaurantId = restaurantId,
                        CompanyId = companyId,
                        Category = category,
                        IsAvailable = isAvailable,
                        IsVegetarian = isVegetarian,
                        IsVegan = isVegan,
                        IsGlutenFree = isGlutenFree
                    }),
                    new CompanyService.Core.Models.Cache.CachePolicy { Expiry = TimeSpan.FromMinutes(10) });

                if (menu == null)
                    return Results.NotFound();

                return Results.Ok(menu);
            }
            catch (Exception ex)
            {
                return Results.BadRequest(new { error = ex.Message });
            }
        }

        private static async Task<IResult> GetMenuCategories(
            Guid companyId,
            Guid restaurantId,
            HttpContext httpContext,
            ISender mediator,
            ICacheService cacheService)
        {
            var claims = httpContext.ExtractTokenClaims();
            if (claims.UserId is null)
                return Results.Unauthorized();

            try
            {
                var cacheKey = $"menu-categories:company:{companyId}:restaurant:{restaurantId}";
                var categories = await cacheService.GetAsync<List<string>>(
                    cacheKey,
                    async () => await mediator.Send(new GetMenuCategoriesQuery
                    {
                        RestaurantId = restaurantId,
                        CompanyId = companyId
                    }),
                    new CompanyService.Core.Models.Cache.CachePolicy { Expiry = TimeSpan.FromMinutes(30) });

                return Results.Ok(categories);
            }
            catch (Exception ex)
            {
                return Results.BadRequest(new { error = ex.Message });
            }
        }

        private static async Task<IResult> SearchMenuItems(
            Guid companyId,
            Guid restaurantId,
            HttpContext httpContext,
            ISender mediator,
            ICacheService cacheService,
            [FromQuery] string searchTerm = "",
            [FromQuery] string? category = null,
            [FromQuery] decimal? minPrice = null,
            [FromQuery] decimal? maxPrice = null)
        {
            var claims = httpContext.ExtractTokenClaims();
            if (claims.UserId is null)
                return Results.Unauthorized();

            try
            {
                var cacheKey = $"search-menu-items:company:{companyId}:restaurant:{restaurantId}:search:{searchTerm}:category:{category ?? "all"}:minPrice:{minPrice?.ToString() ?? "all"}:maxPrice:{maxPrice?.ToString() ?? "all"}";
                var items = await cacheService.GetAsync<List<RestaurantMenuItemDto>>(
                    cacheKey,
                    async () => await mediator.Send(new SearchMenuItemsQuery
                    {
                        RestaurantId = restaurantId,
                        CompanyId = companyId,
                        SearchTerm = searchTerm,
                        Category = category,
                        MinPrice = minPrice,
                        MaxPrice = maxPrice
                    }),
                    new CompanyService.Core.Models.Cache.CachePolicy { Expiry = TimeSpan.FromMinutes(5) });

                return Results.Ok(items);
            }
            catch (Exception ex)
            {
                return Results.BadRequest(new { error = ex.Message });
            }
        }

        // Restaurant Reports Endpoints
        private static async Task<IResult> GetRestaurantReports(
            Guid companyId,
            Guid restaurantId,
            HttpContext httpContext,
            ISender mediator,
            ICacheService cacheService,
            [FromQuery] DateTime? startDate = null,
            [FromQuery] DateTime? endDate = null)
        {
            var claims = httpContext.ExtractTokenClaims();
            if (claims.UserId is null)
                return Results.Unauthorized();

            try
            {
                var cacheKey = $"restaurant-reports:company:{companyId}:restaurant:{restaurantId}:start:{startDate?.ToString("yyyy-MM-dd") ?? "all"}:end:{endDate?.ToString("yyyy-MM-dd") ?? "all"}";
                var reports = await cacheService.GetAsync<RestaurantReportsDto>(
                    cacheKey,
                    async () => await mediator.Send(new GetRestaurantReportsQuery
                    {
                        RestaurantId = restaurantId,
                        CompanyId = companyId,
                        StartDate = startDate,
                        EndDate = endDate
                    }),
                    new CompanyService.Core.Models.Cache.CachePolicy { Expiry = TimeSpan.FromMinutes(5) });

                return Results.Ok(reports);
            }
            catch (Exception ex)
            {
                return Results.BadRequest(new { error = ex.Message });
            }
        }

        private static async Task<IResult> GetRestaurantStats(
            Guid companyId,
            Guid restaurantId,
            HttpContext httpContext,
            ISender mediator,
            ICacheService cacheService)
        {
            var claims = httpContext.ExtractTokenClaims();
            if (claims.UserId is null)
                return Results.Unauthorized();

            try
            {
                var cacheKey = $"restaurant-stats:company:{companyId}:restaurant:{restaurantId}";
                var stats = await cacheService.GetAsync<RestaurantStatsDto>(
                    cacheKey,
                    async () => await mediator.Send(new GetRestaurantStatsQuery
                    {
                        RestaurantId = restaurantId,
                        CompanyId = companyId
                    }),
                    new CompanyService.Core.Models.Cache.CachePolicy { Expiry = TimeSpan.FromMinutes(2) });

                return Results.Ok(stats);
            }
            catch (Exception ex)
            {
                return Results.BadRequest(new { error = ex.Message });
            }
        }

        private static async Task<IResult> GetTableStatus(
            Guid companyId,
            Guid restaurantId,
            HttpContext httpContext,
            ISender mediator,
            ICacheService cacheService)
        {
            var claims = httpContext.ExtractTokenClaims();
            if (claims.UserId is null)
                return Results.Unauthorized();

            try
            {
                var cacheKey = $"table-status:company:{companyId}:restaurant:{restaurantId}";
                var tableStatus = await cacheService.GetAsync<List<TableStatusDto>>(
                    cacheKey,
                    async () => await mediator.Send(new GetTableStatusQuery
                    {
                        RestaurantId = restaurantId,
                        CompanyId = companyId
                    }),
                    new CompanyService.Core.Models.Cache.CachePolicy { Expiry = TimeSpan.FromMinutes(1) });

                return Results.Ok(tableStatus);
            }
            catch (Exception ex)
            {
                return Results.BadRequest(new { error = ex.Message });
            }
        }
    }
}
