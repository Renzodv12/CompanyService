using CompanyService.Core.Entities;
using CompanyService.Core.Feature.Querys.Company;
using CompanyService.Core.Interfaces;
using MediatR;

namespace CompanyService.Core.Feature.Handler.Company
{
    public class GetCompaniesByUserIdQueryHandler : IRequestHandler<GetCompaniesByUserIdQuery, List<CompanyService.Core.Entities.Company>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetCompaniesByUserIdQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<List<CompanyService.Core.Entities.Company>> Handle(GetCompaniesByUserIdQuery request, CancellationToken cancellationToken)
        {
            var userCompanies = await _unitOfWork.Repository<UserCompany>()
                .WhereAsync(uc => uc.UserId == request.UserId);

            var companyIds = userCompanies.Select(uc => uc.CompanyId).Distinct().ToList();

            var companies = await _unitOfWork.Repository<CompanyService.Core.Entities.Company>()
                .WhereAsync(c => companyIds.Contains(c.Id));

            return companies.Select(c => new CompanyService.Core.Entities.Company
            {
                Id = c.Id,
                Name = c.Name,
                RUC = c.RUC
            }).ToList();
        }
    }

}
