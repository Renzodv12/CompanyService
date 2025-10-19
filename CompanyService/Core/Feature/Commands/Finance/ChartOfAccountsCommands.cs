using MediatR;
using CompanyService.Core.DTOs.Finance;

namespace CompanyService.Core.Feature.Commands.Finance
{
    /// <summary>
    /// Comando para crear una nueva cuenta contable
    /// </summary>
    public class CreateChartOfAccountsCommand : IRequest<ChartOfAccountsDto>
    {
        public string Code { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public Guid? ParentId { get; set; }
        public string? Description { get; set; }
        public Guid CompanyId { get; set; }
        public Guid UserId { get; set; }
    }

    /// <summary>
    /// Comando para actualizar una cuenta contable existente
    /// </summary>
    public class UpdateChartOfAccountsCommand : IRequest<ChartOfAccountsDto>
    {
        public Guid Id { get; set; }
        public string? Code { get; set; }
        public string? Name { get; set; }
        public string? Type { get; set; }
        public Guid? ParentId { get; set; }
        public bool? IsActive { get; set; }
        public string? Description { get; set; }
        public Guid CompanyId { get; set; }
        public Guid UserId { get; set; }
    }

    /// <summary>
    /// Comando para eliminar una cuenta contable
    /// </summary>
    public class DeleteChartOfAccountsCommand : IRequest<bool>
    {
        public Guid Id { get; set; }
        public Guid CompanyId { get; set; }
        public Guid UserId { get; set; }
    }
}

