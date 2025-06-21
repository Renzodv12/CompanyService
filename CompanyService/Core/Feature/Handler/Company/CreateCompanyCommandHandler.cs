using CompanyService.Core.Entities;
using CompanyService.Core.Enums;
using CompanyService.Core.Feature.Commands.Company;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;
using CompanyService.Core.Interfaces;
using CompanyService.Core.Exceptions;
namespace CompanyService.Core.Feature.Handler.Company
{

    public class CreateCompanyCommandHandler : IRequestHandler<CreateCompanyCommand, Guid>
    {
        private readonly IUnitOfWork _unitOfWork;

        public CreateCompanyCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Guid> Handle(CreateCompanyCommand request, CancellationToken cancellationToken)
        {
            // Validar existencia empresa con mismo RUC
            var existingCompany = await _unitOfWork.Repository<CompanyService.Core.Entities.Company>()
                .FirstOrDefaultAsync(c => c.RUC == request.RUC);

            if (existingCompany != null)
                throw new DefaultException($"Ya existe una empresa con RUC '{request.RUC}'.");
            // 1. Crear la empresa
            var company = new CompanyService.Core.Entities.Company
            {
                Id = Guid.NewGuid(),
                Name = request.Name,
                RUC = request.RUC,
                CreatedAt = DateTime.UtcNow,
                LastModifiedAt = DateTime.UtcNow
            };

            await _unitOfWork.Repository<CompanyService.Core.Entities.Company>().AddAsync(company);

            // 2. Crear el rol Owner
            var ownerRole = new Role
            {
                Id = Guid.NewGuid(),
                CompanyId = company.Id,
                Name = "Owner"
            };

            await _unitOfWork.Repository<Role>().AddAsync(ownerRole);

            // 3. Asignar el usuario como Owner
            var userCompany = new UserCompany
            {
                Id = Guid.NewGuid(),
                CompanyId = company.Id,
                RoleId = ownerRole.Id,
                UserId = Guid.Parse(request.UserId),
                AssignedAt = DateTime.UtcNow
            };

            await _unitOfWork.Repository<UserCompany>().AddAsync(userCompany);

            // 4. Permisos base
            string[] permissionKeys = new[]
            {
            "Dashboard", "Productos", "Reportes", "Facturacion", "Compra"
        };

            foreach (var key in permissionKeys)
            {
                // Verificamos si ya existe el permiso, si no, lo creamos
                var permission = await _unitOfWork.Repository<Permission>()
                    .FirstOrDefaultAsync(p => p.Key == key);

                if (permission == null)
                {
                    permission = new Permission
                    {
                        Id = Guid.NewGuid(),
                        Key = key,
                        Description = key
                    };

                    await _unitOfWork.Repository<Permission>().AddAsync(permission);
                }

                var rolePermission = new RolePermission
                {
                    Id = Guid.NewGuid(),
                    RoleId = ownerRole.Id,
                    PermissionId = permission.Id,
                    Actions = PermissionAction.View | PermissionAction.Create | PermissionAction.Edit | PermissionAction.Delete
                };

                await _unitOfWork.Repository<RolePermission>().AddAsync(rolePermission);
            }

            await _unitOfWork.SaveChangesAsync();
            return company.Id;
        }
    }
}
