using CompanyService.Core.Interfaces;
using CompanyService.Infrastructure.Context;
using CompanyService.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using StackExchange.Redis;

namespace CompanyService.Infrastructure
{
    public static class Startup
    {
        public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddHealthChecks();
            services.AddDbContext<ApplicationDbContext>(options =>
                     options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));

            services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            
            // InventoryService Repositories
            services.AddScoped<IWarehouseRepository, WarehouseRepository>();
            services.AddScoped<IBatchRepository, BatchRepository>();
            services.AddScoped<IPhysicalInventoryRepository, PhysicalInventoryRepository>();
            services.AddScoped<IReorderPointRepository, ReorderPointRepository>();
            
            // ProcurementService Repositories
            services.AddScoped<IPurchaseOrderRepository, PurchaseOrderRepository>();
            services.AddScoped<IPurchaseOrderItemRepository, PurchaseOrderItemRepository>();
            services.AddScoped<IQuotationRepository, QuotationRepository>();
            services.AddScoped<IQuotationItemRepository, QuotationItemRepository>();
            services.AddScoped<IApprovalRepository, ApprovalRepository>();
            services.AddScoped<IApprovalLevelRepository, ApprovalLevelRepository>();
            services.AddScoped<IApprovalLevelUserRepository, ApprovalLevelUserRepository>();
            services.AddScoped<IGoodsReceiptRepository, GoodsReceiptRepository>();
            services.AddScoped<IGoodsReceiptItemRepository, GoodsReceiptItemRepository>();

            // Configuración de Redis
            services.AddSingleton<IConnectionMultiplexer>(provider =>
            {
                var connectionString = configuration.GetConnectionString("Redis");
                return ConnectionMultiplexer.Connect(connectionString);
            });

            return services;
        }
    }
}


