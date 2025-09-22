using MediatR;
using CompanyService.Core.DTOs.Procurement;

namespace CompanyService.Core.Feature.Querys.Procurement
{
    /// <summary>
    /// Query para obtener una aprobación por ID
    /// </summary>
    public class GetApprovalQuery : IRequest<ApprovalResponse?>
    {
        public Guid Id { get; set; }
        public Guid CompanyId { get; set; }
        public Guid UserId { get; set; }
    }
}