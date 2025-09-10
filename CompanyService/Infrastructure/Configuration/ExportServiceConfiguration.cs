using CompanyService.Core.Interfaces;
using CompanyService.Infrastructure.Services;
using Microsoft.Extensions.DependencyInjection;

namespace CompanyService.Infrastructure.Configuration
{
    public static class ExportServiceConfiguration
    {
        public static IServiceCollection AddExportServices(this IServiceCollection services)
        {
            services.AddScoped<IReportExportService, ReportExportService>();
            
            return services;
        }
    }
}