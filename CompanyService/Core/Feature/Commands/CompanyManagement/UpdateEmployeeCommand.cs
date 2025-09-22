using MediatR;

namespace CompanyService.Core.Feature.Commands.CompanyManagement
{
    public class UpdateEmployeeCommand : IRequest<bool>
    {
        public Guid Id { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public DateTime HireDate { get; set; }
        public decimal Salary { get; set; }
        public Guid PositionId { get; set; }
        public Guid BranchId { get; set; }
        public Guid CompanyId { get; set; }
        public Guid UserId { get; set; }
    }
}