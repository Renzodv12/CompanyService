using MediatR;

namespace CompanyService.Core.Feature.Commands.CRM
{
    /// <summary>
    /// Comando para actualizar un lead existente
    /// </summary>
    public class UpdateLeadCommand : IRequest<bool>
    {
        public Guid Id { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? Phone { get; set; }
        public string? Company { get; set; }
        public string? Position { get; set; }
        public string? Source { get; set; }
        public string Status { get; set; } = string.Empty;
        public string? Notes { get; set; }
        public Guid CompanyId { get; set; }
        public Guid UserId { get; set; }
    }
}