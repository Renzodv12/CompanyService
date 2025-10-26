using CompanyService.Core.Entities;
using CompanyService.Core.Entities.Restaurant;
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
        
        // Finance Module
        public DbSet<AccountsReceivable> AccountsReceivables => Set<AccountsReceivable>();
        public DbSet<AccountsReceivablePayment> AccountsReceivablePayments => Set<AccountsReceivablePayment>();
        public DbSet<AccountsPayable> AccountsPayables => Set<AccountsPayable>();
        public DbSet<AccountsPayablePayment> AccountsPayablePayments => Set<AccountsPayablePayment>();
        public DbSet<BankAccount> BankAccounts => Set<BankAccount>();
        public DbSet<BankTransaction> BankTransactions => Set<BankTransaction>();
        public DbSet<CashFlow> CashFlows => Set<CashFlow>();
        public DbSet<Budget> Budgets => Set<Budget>();
        public DbSet<BudgetLine> BudgetLines => Set<BudgetLine>();
        public DbSet<ChartOfAccounts> ChartOfAccounts => Set<ChartOfAccounts>();
        
        // Restaurant Module
        public DbSet<Restaurant> Restaurants => Set<Restaurant>();
        public DbSet<RestaurantTable> RestaurantTables => Set<RestaurantTable>();
        public DbSet<RestaurantMenu> RestaurantMenus => Set<RestaurantMenu>();
        public DbSet<RestaurantMenuItem> RestaurantMenuItems => Set<RestaurantMenuItem>();
        public DbSet<RestaurantOrder> RestaurantOrders => Set<RestaurantOrder>();
        
        // Advanced Inventory Module
        public DbSet<Warehouse> Warehouses => Set<Warehouse>();
        public DbSet<Batch> Batches => Set<Batch>();
        public DbSet<PhysicalInventory> PhysicalInventories => Set<PhysicalInventory>();
        public DbSet<ReorderPoint> ReorderPoints => Set<ReorderPoint>();
        
        // Procurement Module
        public DbSet<PurchaseOrder> PurchaseOrders => Set<PurchaseOrder>();
        public DbSet<PurchaseOrderItem> PurchaseOrderItems => Set<PurchaseOrderItem>();
        public DbSet<Quotation> Quotations => Set<Quotation>();
        public DbSet<QuotationItem> QuotationItems => Set<QuotationItem>();
        public DbSet<Approval> Approvals => Set<Approval>();
        public DbSet<ApprovalLevel> ApprovalLevels => Set<ApprovalLevel>();
        public DbSet<ApprovalLevelUser> ApprovalLevelUsers => Set<ApprovalLevelUser>();
        public DbSet<GoodsReceipt> GoodsReceipts => Set<GoodsReceipt>();
        public DbSet<GoodsReceiptItem> GoodsReceiptItems => Set<GoodsReceiptItem>();
        
        // CRM Module
        public DbSet<Lead> Leads => Set<Lead>();
        public DbSet<Opportunity> Opportunities => Set<Opportunity>();
        public DbSet<Campaign> Campaigns => Set<Campaign>();
        public DbSet<CampaignLead> CampaignLeads => Set<CampaignLead>();
        public DbSet<CustomerTracking> CustomerTrackings => Set<CustomerTracking>();
        
        // Company Management Module
        public DbSet<Branch> Branches => Set<Branch>();
        public DbSet<Department> Departments => Set<Department>();
        public DbSet<CompanySettings> CompanySettings => Set<CompanySettings>();
        public DbSet<CompanyDocument> CompanyDocuments => Set<CompanyDocument>();
        public DbSet<CompanyBackup> CompanyBackups => Set<CompanyBackup>();
        public DbSet<AuditLog> AuditLogs => Set<AuditLog>();
        
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
