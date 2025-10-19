using CompanyService.Core.Interfaces;
using CompanyService.Core.Services;
using CompanyService.Services;
using CompanyService.Infrastructure.Services;
using CompanyService.Application.Services;
using CompanyService.Core.Validators.Company;
using CompanyService.Core.Validators.Customer;
using CompanyService.Core.Validators.Event;
using CompanyService.Core.Validators.Product;
using CompanyService.Core.Validators.Procurement;
using CompanyService.Core.Validators.Sale;
using CompanyService.Core.Validators.Task;
using CompanyService.Core.Validators.CRM;
using FluentValidation;
using Google.Authenticator;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Reflection;
using System.Text;
using MediatR;

namespace CompanyService.Core
{
    public static class Startup
    {
        public static IServiceCollection AddCoreServices(this IServiceCollection services,  IConfiguration configuration)
        {
            var key = Encoding.ASCII.GetBytes(configuration["JwtConfig:Secret"]);

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo { Title = "CompanyService", Version = "v1" });

                // Agrega la autorización en Swagger
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Description = "JWT Authorization header using the Bearer scheme.",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.Http,
                    Scheme = "Bearer"
                });

                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        new string[] {}
                    }
                });
            });

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ValidateLifetime = false,
                    ValidateIssuerSigningKey = false,
                    ValidIssuer = null,
                    ValidAudience = null,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                };
            });

            services.AddAuthorization();
            services.AddScoped<IReportService, ReportService>();
            services.AddScoped<IDashboardService, DashboardService>();
            services.AddSingleton<IRedisService, RedisService>();
            services.AddScoped<ICacheService, CacheService>();
            services.AddScoped<ICacheInvalidationService, CacheInvalidationService>();
            services.AddScoped<IMenuService, MenuService>();
            
            // InventoryService Services
            services.AddScoped<IWarehouseService, WarehouseService>();
            services.AddScoped<IBatchService, BatchService>();
            services.AddScoped<IPhysicalInventoryService, PhysicalInventoryService>();
            services.AddScoped<IReorderPointService, ReorderPointService>();
            
            // Finance Services
            services.AddScoped<IFinanceService, FinanceService>();
            services.AddScoped<IBankReconciliationService, BankReconciliationService>();
            services.AddScoped<ICashFlowService, CashFlowService>();
            
            // ProcurementService Services
            services.AddScoped<IProcurementService, ProcurementService>();
            
            // CRM Services
            services.AddScoped<ICRMService, CRMService>();
            
            // Dynamic Report Services
            services.AddScoped<IDynamicReportService, DynamicReportService>();
            
            // Company Management Services
            services.AddScoped<ICompanyManagementService, CompanyManagementService>();
            
            services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));
            
            // Registrar el interceptor de cache como comportamiento de pipeline
            services.AddScoped(typeof(IPipelineBehavior<,>), typeof(CompanyService.Core.Interceptors.CacheInvalidationInterceptor<,>));
            services.AddHttpContextAccessor();

            // Registrar TODOS los validadores
            services.AddValidatorsFromAssemblyContaining<CreateCompanyRequestValidator>();
            services.AddValidatorsFromAssemblyContaining<CreateProductRequestValidator>();
            services.AddValidatorsFromAssemblyContaining<UpdateProductRequestValidator>();
            services.AddValidatorsFromAssemblyContaining<CreateCustomerRequestValidator>();
            services.AddValidatorsFromAssemblyContaining<CreateProductCategoryRequestValidator>();
            services.AddValidatorsFromAssemblyContaining<CreateSupplierRequestValidator>();
            services.AddValidatorsFromAssemblyContaining<CreatePurchaseRequestValidator>();
            services.AddValidatorsFromAssemblyContaining<CreateSaleRequestValidator>();
            services.AddValidatorsFromAssemblyContaining<CreateEventRequestValidator>();
            services.AddValidatorsFromAssemblyContaining<UpdateEventRequestValidator>();
            services.AddValidatorsFromAssemblyContaining<CreateTaskRequestValidator>();
            services.AddValidatorsFromAssemblyContaining<UpdateTaskRequestValidator>();
            services.AddValidatorsFromAssemblyContaining<CreateColumnRequestValidator>();
            services.AddValidatorsFromAssemblyContaining<UpdateColumnRequestValidator>();
            services.AddValidatorsFromAssemblyContaining<DragTaskRequestValidator>();
            services.AddValidatorsFromAssemblyContaining<AddCommentRequestValidator>();
            services.AddValidatorsFromAssemblyContaining<CreateSubtaskRequestValidator>();
            services.AddValidatorsFromAssemblyContaining<UpdateSubtaskRequestValidator>();
            
            // Procurement Module Validators
            services.AddValidatorsFromAssemblyContaining<CreatePurchaseOrderRequestValidator>();
            services.AddValidatorsFromAssemblyContaining<CreateQuotationRequestValidator>();
            services.AddValidatorsFromAssemblyContaining<CreateApprovalRequestValidator>();
            services.AddValidatorsFromAssemblyContaining<CreateGoodsReceiptRequestValidator>();
            
            // CRM Module Validators
            services.AddValidatorsFromAssemblyContaining<CreateLeadDtoValidator>();
            services.AddValidatorsFromAssemblyContaining<CreateOpportunityDtoValidator>();
            services.AddValidatorsFromAssemblyContaining<CreateCampaignDtoValidator>();
            
            return services;
        }
    }
 }
