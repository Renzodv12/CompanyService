using MediatR;
using CompanyService.Core.DTOs.Procurement;

namespace CompanyService.Core.Feature.Commands.Procurement
{
    /// <summary>
    /// Command para procesar múltiples aprobaciones en lote
    /// </summary>
    public class BulkProcessApprovalsCommand : IRequest<List<ApprovalResponse>>
    {
        public Guid CompanyId { get; set; }
        public BulkProcessApprovalsRequest Request { get; set; } = new();
    }
}