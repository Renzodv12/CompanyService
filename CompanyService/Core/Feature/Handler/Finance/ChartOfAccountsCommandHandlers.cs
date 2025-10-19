using MediatR;
using CompanyService.Core.Feature.Commands.Finance;
using CompanyService.Core.DTOs.Finance;
using Microsoft.EntityFrameworkCore;
using CompanyService.Infrastructure.Context;
using CompanyService.Core.Entities;

namespace CompanyService.Core.Feature.Handler.Finance
{
    /// <summary>
    /// Handler para crear una nueva cuenta contable
    /// </summary>
    public class CreateChartOfAccountsCommandHandler : IRequestHandler<CreateChartOfAccountsCommand, ChartOfAccountsDto>
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<CreateChartOfAccountsCommandHandler> _logger;

        public CreateChartOfAccountsCommandHandler(
            ApplicationDbContext context,
            ILogger<CreateChartOfAccountsCommandHandler> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<ChartOfAccountsDto> Handle(CreateChartOfAccountsCommand request, CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("Creating chart of accounts entry for company {CompanyId}", request.CompanyId);

                // Validar que el código sea único en la empresa
                var existingCode = await _context.ChartOfAccounts
                    .AnyAsync(c => c.Code == request.Code && c.CompanyId == request.CompanyId, cancellationToken);

                if (existingCode)
                {
                    throw new ArgumentException($"Ya existe una cuenta con el código '{request.Code}' en esta empresa.");
                }

                // Validar que el tipo sea válido
                var validTypes = new[] { "Asset", "Liability", "Equity", "Revenue", "Expense" };
                if (!validTypes.Contains(request.Type))
                {
                    throw new ArgumentException($"Tipo de cuenta inválido. Debe ser uno de: {string.Join(", ", validTypes)}");
                }

                // Validar que el padre exista si se especifica
                if (request.ParentId.HasValue)
                {
                    var parentExists = await _context.ChartOfAccounts
                        .AnyAsync(c => c.Id == request.ParentId.Value && c.CompanyId == request.CompanyId, cancellationToken);

                    if (!parentExists)
                    {
                        throw new ArgumentException("La cuenta padre especificada no existe.");
                    }
                }

                var chartOfAccounts = new ChartOfAccounts
                {
                    Id = Guid.NewGuid(),
                    Code = request.Code,
                    Name = request.Name,
                    Type = request.Type,
                    ParentId = request.ParentId,
                    Description = request.Description,
                    CompanyId = request.CompanyId,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                    IsActive = true
                };

                _context.ChartOfAccounts.Add(chartOfAccounts);
                await _context.SaveChangesAsync(cancellationToken);

                // Recargar con relaciones
                var createdAccount = await _context.ChartOfAccounts
                    .Include(c => c.Parent)
                    .Where(c => c.Id == chartOfAccounts.Id && c.CompanyId == request.CompanyId)
                    .Select(c => new ChartOfAccountsDto
                    {
                        Id = c.Id,
                        Code = c.Code,
                        Name = c.Name,
                        Type = c.Type,
                        ParentId = c.ParentId,
                        ParentName = c.Parent != null ? c.Parent.Name : null,
                        IsActive = c.IsActive,
                        Description = c.Description,
                        CreatedAt = c.CreatedAt,
                        UpdatedAt = c.UpdatedAt
                    })
                    .FirstOrDefaultAsync(cancellationToken);

                if (createdAccount == null)
                {
                    throw new InvalidOperationException("No se pudo recuperar la cuenta contable creada.");
                }

                _logger.LogInformation("Chart of accounts entry {AccountId} created successfully for company {CompanyId}",
                    chartOfAccounts.Id, request.CompanyId);

                return createdAccount;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating chart of accounts entry for company {CompanyId}", request.CompanyId);
                throw;
            }
        }
    }

    /// <summary>
    /// Handler para actualizar una cuenta contable existente
    /// </summary>
    public class UpdateChartOfAccountsCommandHandler : IRequestHandler<UpdateChartOfAccountsCommand, ChartOfAccountsDto>
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<UpdateChartOfAccountsCommandHandler> _logger;

        public UpdateChartOfAccountsCommandHandler(
            ApplicationDbContext context,
            ILogger<UpdateChartOfAccountsCommandHandler> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<ChartOfAccountsDto> Handle(UpdateChartOfAccountsCommand request, CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("Updating chart of accounts entry {AccountId} for company {CompanyId}",
                    request.Id, request.CompanyId);

                var chartOfAccounts = await _context.ChartOfAccounts
                    .FirstOrDefaultAsync(c => c.Id == request.Id && c.CompanyId == request.CompanyId, cancellationToken);

                if (chartOfAccounts == null)
                {
                    throw new ArgumentException("Cuenta contable no encontrada");
                }

                // Validar que el código sea único si se está cambiando
                if (!string.IsNullOrEmpty(request.Code) && request.Code != chartOfAccounts.Code)
                {
                    var existingCode = await _context.ChartOfAccounts
                        .AnyAsync(c => c.Code == request.Code && c.CompanyId == request.CompanyId && c.Id != request.Id, cancellationToken);

                    if (existingCode)
                    {
                        throw new ArgumentException($"Ya existe una cuenta con el código '{request.Code}' en esta empresa.");
                    }
                    chartOfAccounts.Code = request.Code;
                }

                // Validar que el tipo sea válido si se está cambiando
                if (!string.IsNullOrEmpty(request.Type))
                {
                    var validTypes = new[] { "Asset", "Liability", "Equity", "Revenue", "Expense" };
                    if (!validTypes.Contains(request.Type))
                    {
                        throw new ArgumentException($"Tipo de cuenta inválido. Debe ser uno de: {string.Join(", ", validTypes)}");
                    }
                    chartOfAccounts.Type = request.Type;
                }

                // Validar que el padre exista si se especifica
                if (request.ParentId.HasValue)
                {
                    var parentExists = await _context.ChartOfAccounts
                        .AnyAsync(c => c.Id == request.ParentId.Value && c.CompanyId == request.CompanyId, cancellationToken);

                    if (!parentExists)
                    {
                        throw new ArgumentException("La cuenta padre especificada no existe.");
                    }
                    chartOfAccounts.ParentId = request.ParentId;
                }

                // Actualizar otros campos
                if (!string.IsNullOrEmpty(request.Name))
                    chartOfAccounts.Name = request.Name;

                if (request.IsActive.HasValue)
                    chartOfAccounts.IsActive = request.IsActive.Value;

                if (request.Description != null)
                    chartOfAccounts.Description = request.Description;

                chartOfAccounts.UpdatedAt = DateTime.UtcNow;

                await _context.SaveChangesAsync(cancellationToken);

                // Recargar con relaciones
                var updatedAccount = await _context.ChartOfAccounts
                    .Include(c => c.Parent)
                    .Where(c => c.Id == request.Id && c.CompanyId == request.CompanyId)
                    .Select(c => new ChartOfAccountsDto
                    {
                        Id = c.Id,
                        Code = c.Code,
                        Name = c.Name,
                        Type = c.Type,
                        ParentId = c.ParentId,
                        ParentName = c.Parent != null ? c.Parent.Name : null,
                        IsActive = c.IsActive,
                        Description = c.Description,
                        CreatedAt = c.CreatedAt,
                        UpdatedAt = c.UpdatedAt
                    })
                    .FirstOrDefaultAsync(cancellationToken);

                if (updatedAccount == null)
                {
                    throw new InvalidOperationException("No se pudo recuperar la cuenta contable actualizada.");
                }

                _logger.LogInformation("Chart of accounts entry {AccountId} updated successfully for company {CompanyId}",
                    request.Id, request.CompanyId);

                return updatedAccount;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating chart of accounts entry {AccountId} for company {CompanyId}",
                    request.Id, request.CompanyId);
                throw;
            }
        }
    }

    /// <summary>
    /// Handler para eliminar una cuenta contable
    /// </summary>
    public class DeleteChartOfAccountsCommandHandler : IRequestHandler<DeleteChartOfAccountsCommand, bool>
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<DeleteChartOfAccountsCommandHandler> _logger;

        public DeleteChartOfAccountsCommandHandler(
            ApplicationDbContext context,
            ILogger<DeleteChartOfAccountsCommandHandler> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<bool> Handle(DeleteChartOfAccountsCommand request, CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("Deleting chart of accounts entry {AccountId} for company {CompanyId}",
                    request.Id, request.CompanyId);

                var chartOfAccounts = await _context.ChartOfAccounts
                    .Include(c => c.Children)
                    .FirstOrDefaultAsync(c => c.Id == request.Id && c.CompanyId == request.CompanyId, cancellationToken);

                if (chartOfAccounts == null)
                {
                    throw new ArgumentException("Cuenta contable no encontrada");
                }

                // Verificar si tiene cuentas hijas
                if (chartOfAccounts.Children.Any())
                {
                    throw new InvalidOperationException("No se puede eliminar una cuenta que tiene cuentas hijas. Primero elimine las cuentas hijas.");
                }

                // Verificar si está siendo usada en presupuestos
                var hasBudgetLines = await _context.BudgetLines
                    .AnyAsync(bl => bl.AccountId == request.Id, cancellationToken);

                if (hasBudgetLines)
                {
                    throw new InvalidOperationException("No se puede eliminar una cuenta que está siendo usada en presupuestos.");
                }

                _context.ChartOfAccounts.Remove(chartOfAccounts);
                await _context.SaveChangesAsync(cancellationToken);

                _logger.LogInformation("Chart of accounts entry {AccountId} deleted successfully for company {CompanyId}",
                    request.Id, request.CompanyId);

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting chart of accounts entry {AccountId} for company {CompanyId}",
                    request.Id, request.CompanyId);
                throw;
            }
        }
    }
}
