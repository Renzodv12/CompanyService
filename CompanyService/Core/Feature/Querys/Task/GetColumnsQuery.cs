using CompanyService.Core.Models.Task;
using MediatR;

namespace CompanyService.Core.Feature.Querys.Task
{
    public class GetColumnsQuery : IRequest<List<ColumnDto>>
    {
        public Guid CompanyId { get; set; }
        public string UserId { get; set; }
    }
}
