using MediatR;
using Microsoft.EntityFrameworkCore;
using CompanyService.Core.DTOs.Restaurant;
using CompanyService.Core.Entities.Restaurant;

using CompanyService.Core.Interfaces;
using CompanyService.Core.Feature.Querys.Restaurant;

namespace CompanyService.Core.Feature.Handler.Restaurant
{
    public class GetMenuCategoriesQueryHandler : IRequestHandler<GetMenuCategoriesQuery, List<string>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<GetMenuCategoriesQueryHandler> _logger;

        public GetMenuCategoriesQueryHandler(IUnitOfWork unitOfWork, ILogger<GetMenuCategoriesQueryHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<List<string>> Handle(GetMenuCategoriesQuery request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Getting menu categories for restaurant {RestaurantId}", request.RestaurantId);

            var menuItems = await _unitOfWork.Repository<RestaurantMenuItem>()
                .WhereAsync(mi => mi.RestaurantId == request.RestaurantId && mi.IsAvailable);

            var categories = menuItems
                .Where(mi => !string.IsNullOrEmpty(mi.Category))
                .Select(mi => mi.Category!)
                .Distinct()
                .OrderBy(c => c)
                .ToList();

            return categories;
        }
    }
}
