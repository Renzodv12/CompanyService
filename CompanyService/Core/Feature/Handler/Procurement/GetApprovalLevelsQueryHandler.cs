using CompanyService.Core.DTOs;
using CompanyService.Core.DTOs.Procurement;
using CompanyService.Core.Entities;
using CompanyService.Core.Feature.Querys.Procurement;
using CompanyService.Core.Interfaces;
using MediatR;

namespace CompanyService.Core.Feature.Handler.Procurement
{
    /// <summary>
    /// Handler para obtener los niveles de aprobación
    /// </summary>
    public class GetApprovalLevelsQueryHandler : IRequestHandler<GetApprovalLevelsQuery, PagedResult<ApprovalLevelResponse>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetApprovalLevelsQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<PagedResult<ApprovalLevelResponse>> Handle(GetApprovalLevelsQuery request, CancellationToken cancellationToken)
        {
            var allApprovalLevels = await _unitOfWork.Repository<ApprovalLevel>()
                .WhereAsync(al => al.CompanyId == request.CompanyId);

            // Obtener total de registros
            var totalCount = allApprovalLevels.Count();

            // Aplicar paginación
            var approvalLevels = allApprovalLevels
                .OrderBy(al => al.Level)
                .Skip((request.Page - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToList();

            // Mapear a Response DTOs
            var approvalLevelResponses = approvalLevels.Select(al => new ApprovalLevelResponse
            {
                Id = al.Id,
                Name = al.Name,
                Level = al.Level,
                MinAmount = al.MinAmount,
                MaxAmount = al.MaxAmount,
                RequiresAllApprovers = al.RequiresAllApprovers,
                IsActive = al.IsActive,
                CompanyId = al.CompanyId
            }).ToList();

            return new PagedResult<ApprovalLevelResponse>
            {
                Items = approvalLevelResponses,
                TotalCount = totalCount,
                PageNumber = request.Page,
                PageSize = request.PageSize
            };
        }
    }
}