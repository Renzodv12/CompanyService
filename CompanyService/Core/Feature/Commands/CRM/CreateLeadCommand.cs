using MediatR;

namespace CompanyService.Core.Feature.Commands.CRM
{
    /// <summary>
    /// Comando para crear un nuevo lead
    /// </summary>
    public class CreateLeadCommand : IRequest<Guid>
    {
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