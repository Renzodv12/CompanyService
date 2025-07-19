using CompanyService.Core.Interfaces;
using CompanyService.Core.Services;
using CompanyService.Core.Validators.Company;
using CompanyService.Core.Validators.Customer;
using CompanyService.Core.Validators.Product;
using CompanyService.Core.Validators.Sale;
using FluentValidation;
using Google.Authenticator;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Reflection;
using System.Text;

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
            services.AddScoped<IRedisService, RedisService>();
            services.AddScoped<IReportService, ReportService>();
            services.AddScoped<IDashboardService, DashboardService>();
            services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));
            services.AddHttpContextAccessor();

            // Registrar TODOS los validadores
            services.AddValidatorsFromAssemblyContaining<CreateCompanyRequestValidator>();
            services.AddValidatorsFromAssemblyContaining<CreateProductRequestValidator>();
            services.AddValidatorsFromAssemblyContaining<CreateCustomerRequestValidator>();
            services.AddValidatorsFromAssemblyContaining<CreateProductCategoryRequestValidator>();
            services.AddValidatorsFromAssemblyContaining<CreateSupplierRequestValidator>();
            services.AddValidatorsFromAssemblyContaining<CreatePurchaseRequestValidator>();
            services.AddValidatorsFromAssemblyContaining<CreateSaleRequestValidator>();
            return services;
        }
    }
 }
