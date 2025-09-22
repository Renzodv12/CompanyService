using MediatR;
using CompanyService.Core.DTOs;

namespace CompanyService.Core.Feature.Querys.CompanyManagement
{
    /// <summary>
    /// Query para obtener la lista de posiciones
    /// </summary>
    public class GetPositionsQuery : IRequest<List<PositionDto>>
    {
        public Guid CompanyId { get; set; }
        public Guid UserId { get; set; }
    }
}