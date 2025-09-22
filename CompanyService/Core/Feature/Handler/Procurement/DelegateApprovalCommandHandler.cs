using CompanyService.Core.Entities;
using CompanyService.Core.Feature.Commands.Procurement;
using CompanyService.Core.Interfaces;
using CompanyService.Core.Exceptions;
using MediatR;

namespace CompanyService.Core.Feature.Handler.Procurement
{
    /// <summary>
    /// Handler para delegar aprobaciones
    /// </summary>
    public class DelegateApprovalCommandHandler : IRequestHandler<DelegateApprovalCommand, bool>
    {
        private readonly IUnitOfWork _unitOfWork;

        public DelegateApprovalCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<bool> Handle(DelegateApprovalCommand request, CancellationToken cancellationToken)
        {
            // Verificar que el usuario origen existe
            var fromUser = await _unitOfWork.Repository<User>()
                .FirstOrDefaultAsync(u => u.Id == request.FromUserId);

            if (fromUser == null)
                throw new DefaultException("Usuario origen no encontrado.");

            // Verificar que el usuario destino existe
            var toUser = await _unitOfWork.Repository<User>()
                .FirstOrDefaultAsync(u => u.Id == request.ToUserId);

            if (toUser == null)
                throw new DefaultException("Usuario destino no encontrado.");

            // Si se especifican niveles de aprobación específicos, actualizar esos niveles
            if (request.ApprovalLevelIds != null && request.ApprovalLevelIds.Any())
            {
                var approvalLevelUsers = await _unitOfWork.Repository<ApprovalLevelUser>()
                    .WhereAsync(alu => request.ApprovalLevelIds.Contains(alu.ApprovalLevelId) && 
                                      alu.UserId == request.FromUserId);

                foreach (var approvalLevelUser in approvalLevelUsers)
                {
                    approvalLevelUser.DelegateUserId = request.ToUserId;
                    approvalLevelUser.DelegateFromDate = request.StartDate;
                    approvalLevelUser.DelegateToDate = request.EndDate;
                    approvalLevelUser.DelegateReason = request.Comments;
                    approvalLevelUser.ModifiedDate = DateTime.UtcNow;
                    
                    _unitOfWork.Repository<ApprovalLevelUser>().Update(approvalLevelUser);
                }
            }
            else
            {
                // Si no se especifican niveles, delegar todas las aprobaciones del usuario
                var allApprovalLevelUsers = await _unitOfWork.Repository<ApprovalLevelUser>()
                    .WhereAsync(alu => alu.UserId == request.FromUserId);

                foreach (var approvalLevelUser in allApprovalLevelUsers)
                {
                    approvalLevelUser.DelegateUserId = request.ToUserId;
                    approvalLevelUser.DelegateFromDate = request.StartDate;
                    approvalLevelUser.DelegateToDate = request.EndDate;
                    approvalLevelUser.DelegateReason = request.Comments;
                    approvalLevelUser.ModifiedDate = DateTime.UtcNow;
                    
                    _unitOfWork.Repository<ApprovalLevelUser>().Update(approvalLevelUser);
                }
            }

            await _unitOfWork.SaveChangesAsync();
            return true;
        }
    }
}