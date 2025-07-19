using MediatR;

namespace CompanyService.Core.Feature.Commands.Product
{
    public class CreateProductCategoryCommand : IRequest<Guid>
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public Guid CompanyId { get; set; }
        public string UserId { get; set; }
    }
}
