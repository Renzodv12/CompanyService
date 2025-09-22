using CompanyService.WebApi.Endpoints;

namespace CompanyService.WebApi.Extensions
{
    public static class EndpointExtensions
    {
        public static IEndpointRouteBuilder MapAllEndpoints(this IEndpointRouteBuilder app)
        {
            // Mapear endpoints de roles
            app.MapRoleEndpoints();
            
            // Mapear endpoints de permisos
            app.MapPermissionEndpoints();
            
            // Mapear endpoints de usuarios
            app.MapUserEndpoints();

            return app;
        }
    }
}