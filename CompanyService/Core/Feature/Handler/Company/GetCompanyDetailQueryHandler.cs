using CompanyService.Core.Entities;
using CompanyService.Core.Feature.Querys.Company;
using CompanyService.Core.Interfaces;
using CompanyService.Core.Models.Company;
using CompanyService.Core.Utils;
using MediatR;

namespace CompanyService.Core.Feature.Handler.Company
{
    public class GetCompanyDetailQueryHandler : IRequestHandler<GetCompanyDetailQuery, CompanyWithPermissionsDto?>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetCompanyDetailQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<CompanyWithPermissionsDto?> Handle(GetCompanyDetailQuery request, CancellationToken cancellationToken)
        {
            // Validar acceso
            var userCompany = await _unitOfWork.Repository<UserCompany>()
                .FirstOrDefaultAsync(uc => uc.UserId == request.UserId && uc.CompanyId == request.CompanyId);

            if (userCompany is null)
                return null;

            var company = await _unitOfWork.Repository<CompanyService.Core.Entities.Company>().GetByIdAsync(request.CompanyId);
            if (company is null)
                return null;

            var role = await _unitOfWork.Repository<Role>().GetByIdAsync(userCompany.RoleId);
            if (role is null)
                return null;

            var rolePermissions = await _unitOfWork.Repository<RolePermission>()
                .WhereAsync(rp => rp.RoleId == role.Id);

            var permissionIds = rolePermissions.Select(rp => rp.PermissionId).ToList();
            var permissions = await _unitOfWork.Repository<Permission>()
                .WhereAsync(p => permissionIds.Contains(p.Id));

            var result = new CompanyWithPermissionsDto
            {
                Id = company.Id,
                Name = company.Name,
                RUC = company.RUC,
                Role = role.Name,
                Permissions = permissions.Select(p =>
                {
                    var actions = rolePermissions.First(rp => rp.PermissionId == p.Id).Actions;
                    return new PermissionDto
                    {
                        Key = p.Key,
                        Description = p.Description,
                        Actions = ((int)actions).GetPermissionsNames()
                    };
                }).ToList()
            };

            return result;
        }
    }
 }