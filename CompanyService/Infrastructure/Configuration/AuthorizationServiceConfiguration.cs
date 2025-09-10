using CompanyService.Core.Interfaces;
using CompanyService.Infrastructure.Services;
using Microsoft.Extensions.DependencyInjection;

namespace CompanyService.Infrastructure.Configuration
{
    /// <summary>
    /// Configuración de servicios de autorización para reportes dinámicos
    /// </summary>
    public static class AuthorizationServiceConfiguration
    {
        /// <summary>
        /// Registra los servicios de autorización y auditoría en el contenedor de dependencias
        /// </summary>
        /// <param name="services">Colección de servicios</param>
        /// <returns>Colección de servicios configurada</returns>
        public static IServiceCollection AddReportAuthorizationServices(this IServiceCollection services)
        {
            services.AddScoped<IReportAuthorizationService, ReportAuthorizationService>();
            services.AddScoped<IReportAuditService, ReportAuditService>();
            
            return services;
        }
    }
}