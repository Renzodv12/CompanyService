using MediatR;
using CompanyService.Core.Interfaces;
using CompanyService.Core.Services;

namespace CompanyService.Core.Interceptors
{
    /// <summary>
    /// Interceptor de MediatR para invalidar cache automáticamente cuando se modifican datos
    /// </summary>
    public class CacheInvalidationInterceptor<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
        where TRequest : IRequest<TResponse>
    {
        private readonly ICacheInvalidationService _cacheInvalidationService;
        private readonly ILogger<CacheInvalidationInterceptor<TRequest, TResponse>> _logger;

        public CacheInvalidationInterceptor(
            ICacheInvalidationService cacheInvalidationService,
            ILogger<CacheInvalidationInterceptor<TRequest, TResponse>> logger)
        {
            _cacheInvalidationService = cacheInvalidationService;
            _logger = logger;
        }

        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            // Ejecutar el handler
            var response = await next();

            // Invalidar cache basado en el tipo de request
            await InvalidateCacheForRequest(request);

            return response;
        }

        private async Task InvalidateCacheForRequest(TRequest request)
        {
            try
            {
                var requestType = request.GetType().Name;
                var dataType = GetDataTypeFromRequest(requestType);
                
                if (string.IsNullOrEmpty(dataType))
                    return;

                // Obtener companyId del request si está disponible
                var companyId = ExtractCompanyId(request);
                if (companyId.HasValue)
                {
                    await _cacheInvalidationService.InvalidateCompanyDataAsync(companyId.Value, dataType);
                    _logger.LogDebug("Invalidated cache for company {CompanyId} and data type {DataType}", companyId.Value, dataType);
                }
                else
                {
                    // Invalidar por patrón general
                    await _cacheInvalidationService.InvalidatePatternAsync($"*{dataType}*");
                    _logger.LogDebug("Invalidated cache pattern for data type {DataType}", dataType);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error invalidating cache for request {RequestType}", request.GetType().Name);
            }
        }

        private string GetDataTypeFromRequest(string requestType)
        {
            return requestType switch
            {
                var type when type.Contains("Product") => CacheDataTypes.Products,
                var type when type.Contains("Customer") => CacheDataTypes.Customers,
                var type when type.Contains("Sale") => CacheDataTypes.Sales,
                var type when type.Contains("Purchase") => CacheDataTypes.Purchases,
                var type when type.Contains("Lead") => CacheDataTypes.Leads,
                var type when type.Contains("Opportunity") => CacheDataTypes.Opportunities,
                var type when type.Contains("Budget") => CacheDataTypes.Budgets,
                var type when type.Contains("Company") => CacheDataTypes.Companies,
                var type when type.Contains("Task") => CacheDataTypes.Tasks,
                var type when type.Contains("Dashboard") => CacheDataTypes.Dashboard,
                _ => string.Empty
            };
        }

        private Guid? ExtractCompanyId(TRequest request)
        {
            try
            {
                var companyIdProperty = request.GetType().GetProperty("CompanyId");
                if (companyIdProperty != null && companyIdProperty.GetValue(request) is Guid companyId)
                {
                    return companyId;
                }

                // Buscar en propiedades anidadas
                var properties = request.GetType().GetProperties();
                foreach (var prop in properties)
                {
                    var value = prop.GetValue(request);
                    if (value != null)
                    {
                        var nestedCompanyIdProp = value.GetType().GetProperty("CompanyId");
                        if (nestedCompanyIdProp != null && nestedCompanyIdProp.GetValue(value) is Guid nestedCompanyId)
                        {
                            return nestedCompanyId;
                        }
                    }
                }

                return null;
            }
            catch
            {
                return null;
            }
        }
    }
}
