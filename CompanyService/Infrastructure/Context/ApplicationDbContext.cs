using CompanyService.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace CompanyService.Infrastructure.Context
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options) { }


        public DbSet<User> Users => Set<User>();
        public DbSet<Company> Companies => Set<Company>();
        public DbSet<Role> Roles => Set<Role>();
        public DbSet<UserCompany> UserCompanys => Set<UserCompany>();
        public DbSet<RolePermission> RolePermissions => Set<RolePermission>();
        public DbSet<Permission> Permissions => Set<Permission>();
        // Inventario
        public DbSet<Product> Products => Set<Product>();
        public DbSet<ProductCategory> ProductCategories => Set<ProductCategory>();
        public DbSet<StockMovement> StockMovements => Set<StockMovement>();

        // Ventas
        public DbSet<Customer> Customers => Set<Customer>();
        public DbSet<Sale> Sales => Set<Sale>();
        public DbSet<SaleDetail> SaleDetails => Set<SaleDetail>();

        // Compras
        public DbSet<Supplier> Suppliers => Set<Supplier>();
        public DbSet<Purchase> Purchases => Set<Purchase>();
        public DbSet<PurchaseDetail> PurchaseDetails => Set<PurchaseDetail>();

        // Contabilidad
        public DbSet<Account> Accounts => Set<Account>();
        public DbSet<JournalEntry> JournalEntries => Set<JournalEntry>();
        public DbSet<Tax> Taxes => Set<Tax>();

        // Reportes
        public DbSet<Report> Reports => Set<Report>();
        public DbSet<ReportExecution> ReportExecutions => Set<ReportExecution>();
        
        // Reportes Dinámicos
        public DbSet<ReportDefinition> ReportDefinitions => Set<ReportDefinition>();
        public DbSet<ReportField> ReportFields => Set<ReportField>();
        public DbSet<ReportFilter> ReportFilters => Set<ReportFilter>();
        public DbSet<DynamicReportExecution> DynamicReportExecutions => Set<DynamicReportExecution>();
        public DbSet<ReportAuditLog> ReportAuditLogs => Set<ReportAuditLog>();

        //Events
        public DbSet<Event> Events => Set<Event>();
        public DbSet<EventAttendee> EventAttendees => Set<EventAttendee>();
        // Tasks
        public DbSet<CompanyService.Core.Entities.Task> Tasks => Set<CompanyService.Core.Entities.Task>();
        public DbSet<TaskColumn> TaskColumns => Set<TaskColumn>();
        public DbSet<TaskLabel> TaskLabels => Set<TaskLabel>();
        public DbSet<TaskAssignee> TaskAssignees => Set<TaskAssignee>();
        public DbSet<TaskAttachment> TaskAttachments => Set<TaskAttachment>();
        public DbSet<TaskSubtask> TaskSubtasks => Set<TaskSubtask>();
        public DbSet<TaskComment> TaskComments => Set<TaskComment>();
        
        // Menús
        public DbSet<Menu> Menus => Set<Menu>();
        public DbSet<CompanyMenuConfiguration> CompanyMenuConfigurations => Set<CompanyMenuConfiguration>();
        
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
            
            // Configure DateTime properties to use UTC for PostgreSQL compatibility
            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                foreach (var property in entityType.GetProperties())
                {
                    if (property.ClrType == typeof(DateTime) || property.ClrType == typeof(DateTime?))
                    {
                        property.SetValueConverter(new Microsoft.EntityFrameworkCore.Storage.ValueConversion.ValueConverter<DateTime, DateTime>(
                            v => v.Kind == DateTimeKind.Unspecified ? DateTime.SpecifyKind(v, DateTimeKind.Utc) : v.ToUniversalTime(),
                            v => DateTime.SpecifyKind(v, DateTimeKind.Utc)));
                    }
                }
            }
            
            base.OnModelCreating(modelBuilder);
        }
    }
}
