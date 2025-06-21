using MediatR;

namespace CompanyService.Core.Feature.Commands.Company
{
    public class CreateCompanyCommand : IRequest<Guid> // retorna el Id de la empresa creada
    {
        public string Name { get; set; }
        public string RUC { get; set; }
        public string UserId { get; set; } // extraído del token en el endpoint
    }
}
