using CompanyService.Core.Entities;
using CompanyService.Core.Feature.Querys.Task;
using CompanyService.Core.Interfaces;
using CompanyService.Core.Models.Task;
using MediatR;

namespace CompanyService.Core.Feature.Handler.Task
{
    public class GetColumnsQueryHandler : IRequestHandler<GetColumnsQuery, List<ColumnDto>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetColumnsQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<List<ColumnDto>> Handle(GetColumnsQuery request, CancellationToken cancellationToken)
        {
            var userCompany = await _unitOfWork.Repository<UserCompany>()
                .FirstOrDefaultAsync(uc => uc.UserId == Guid.Parse(request.UserId) && uc.CompanyId == request.CompanyId);

            if (userCompany == null)
                return new List<ColumnDto>();

            var columns = await _unitOfWork.Repository<TaskColumn>()
                .WhereAsync(c => c.CompanyId == request.CompanyId);

            var tasks = await _unitOfWork.Repository<CompanyService.Core.Entities.Task>()
                .WhereAsync(t => t.CompanyId == request.CompanyId);

            return columns
                .OrderBy(c => c.DisplayOrder)
                .Select(c => new ColumnDto
                {
                    Id = c.Id.ToString(),
                    Name = c.Name,
                    TaskIds = tasks.Where(t => t.ColumnId == c.Id)
                                  .OrderBy(t => t.CreatedAt)
                                  .Select(t => t.Id.ToString())
                                  .ToList()
                }).ToList();
        }
    }
}
