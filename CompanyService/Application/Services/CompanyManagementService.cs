using Microsoft.EntityFrameworkCore;
using CompanyService.Core.Interfaces;
using CompanyService.Core.DTOs.Branch;
using CompanyService.Core.DTOs.Department;
using CompanyService.Core.DTOs.CompanySettings;
using CompanyService.Core.DTOs.CompanyDocument;
using CompanyService.Core.Entities;
using CompanyService.Core.Enums;
using CompanyService.Infrastructure.Context;
using System.Text.Json;

namespace CompanyService.Application.Services
{
    /// <summary>
    /// Servicio para gestión avanzada de empresas
    /// </summary>
    public class CompanyManagementService : ICompanyManagementService
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<CompanyManagementService> _logger;
        private readonly string _documentsPath;

        public CompanyManagementService(
            ApplicationDbContext context,
            ILogger<CompanyManagementService> logger,
            IConfiguration configuration)
        {
            _context = context;
            _logger = logger;
            _documentsPath = configuration.GetValue<string>("DocumentsPath") ?? "Documents";
        }

        #region Branch Management
        public async System.Threading.Tasks.Task<IEnumerable<BranchDto>> GetBranchesAsync(Guid companyId)
        {
            var branches = await _context.Branches
                .Where(b => b.CompanyId == companyId && !b.IsDeleted)
                .Include(b => b.Manager)
                .OrderBy(b => b.Name)
                .ToListAsync();

            return branches.Select(MapToBranchDto);
        }

        public async System.Threading.Tasks.Task<BranchDto?> GetBranchByIdAsync(Guid companyId, Guid branchId)
        {
            var branch = await _context.Branches
                .Where(b => b.Id == branchId && b.CompanyId == companyId && !b.IsDeleted)
                .Include(b => b.Manager)
                .FirstOrDefaultAsync();

            return branch != null ? MapToBranchDto(branch) : null;
        }

        public async System.Threading.Tasks.Task<Guid> CreateBranchAsync(Guid companyId, CreateBranchRequest request, Guid userId)
        {
            var branch = new Branch
            {
                Id = Guid.NewGuid(),
                CompanyId = companyId,
                Name = request.Name,
                Address = request.Address,
                City = request.City,
                State = request.State,
                Country = request.Country,
                PostalCode = request.PostalCode,
                Phone = request.Phone,
                Email = request.Email,
                ManagerId = request.ManagerId,
                IsActive = request.IsActive,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = userId
            };

            _context.Branches.Add(branch);
            await _context.SaveChangesAsync();

            await LogAuditActionAsync(companyId, userId, "CREATE", "Branch", branch.Id, 
                branch.Name, null, branch, "", "");

            return branch.Id;
        }

        public async System.Threading.Tasks.Task UpdateBranchAsync(Guid companyId, Guid branchId, UpdateBranchRequest request, Guid userId)
        {
            var branch = await _context.Branches
                .FirstOrDefaultAsync(b => b.Id == branchId && b.CompanyId == companyId && !b.IsDeleted);

            if (branch == null)
                throw new InvalidOperationException("Sucursal no encontrada");

            var oldValues = new
            {
                branch.Name,
                branch.Address,
                branch.City,
                branch.State,
                branch.Country,
                branch.PostalCode,
                branch.Phone,
                branch.Email,
                branch.ManagerId,
                branch.IsActive
            };

            branch.Name = request.Name;
            branch.Address = request.Address;
            branch.City = request.City;
            branch.State = request.State;
            branch.Country = request.Country;
            branch.PostalCode = request.PostalCode;
            branch.Phone = request.Phone;
            branch.Email = request.Email;
            branch.ManagerId = request.ManagerId;
            if (request.IsActive.HasValue)
                branch.IsActive = request.IsActive.Value;
            branch.UpdatedAt = DateTime.UtcNow;
            branch.UpdatedBy = userId;

            await _context.SaveChangesAsync();

            await LogAuditActionAsync(companyId, userId, "UPDATE", "Branch", branch.Id, 
                branch.Name, oldValues, branch, "", "");
        }

        public async System.Threading.Tasks.Task DeleteBranchAsync(Guid companyId, Guid branchId, Guid userId)
        {
            var branch = await _context.Branches
                .FirstOrDefaultAsync(b => b.Id == branchId && b.CompanyId == companyId && !b.IsDeleted);

            if (branch == null)
                throw new InvalidOperationException("Sucursal no encontrada");

            branch.IsDeleted = true;
            branch.UpdatedAt = DateTime.UtcNow;
            branch.UpdatedBy = userId;

            await _context.SaveChangesAsync();

            await LogAuditActionAsync(companyId, userId, "DELETE", "Branch", branch.Id, 
                branch.Name, branch, null, "", "");
        }
        #endregion

        #region Department Management
        public async System.Threading.Tasks.Task<IEnumerable<DepartmentDto>> GetDepartmentsAsync(Guid companyId)
        {
            var departments = await _context.Departments
                .Where(d => d.CompanyId == companyId && !d.IsDeleted)
                .Include(d => d.Manager)
                .Include(d => d.Branch)
                .Include(d => d.ParentDepartment)
                .OrderBy(d => d.Name)
                .ToListAsync();

            return departments.Select(MapToDepartmentDto);
        }

        public async System.Threading.Tasks.Task<DepartmentDto?> GetDepartmentByIdAsync(Guid companyId, Guid departmentId)
        {
            var department = await _context.Departments
                .Where(d => d.Id == departmentId && d.CompanyId == companyId && !d.IsDeleted)
                .Include(d => d.Manager)
                .Include(d => d.Branch)
                .Include(d => d.ParentDepartment)
                .FirstOrDefaultAsync();

            return department != null ? MapToDepartmentDto(department) : null;
        }

        public async System.Threading.Tasks.Task<Guid> CreateDepartmentAsync(Guid companyId, CreateDepartmentRequest request, Guid userId)
        {
            var department = new Department
            {
                Id = Guid.NewGuid(),
                CompanyId = companyId,
                Name = request.Name,
                BranchId = request.BranchId,
                Description = request.Description,
                ManagerId = request.ManagerId,
                ParentDepartmentId = request.ParentDepartmentId,
                Budget = request.Budget,
                Code = request.Code,
                IsActive = request.IsActive,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = userId
            };

            _context.Departments.Add(department);
            await _context.SaveChangesAsync();

            await LogAuditActionAsync(companyId, userId, "CREATE", "Department", department.Id, 
                department.Name, null, department, "", "");

            return department.Id;
        }

        public async System.Threading.Tasks.Task UpdateDepartmentAsync(Guid companyId, Guid departmentId, UpdateDepartmentRequest request, Guid userId)
        {
            var department = await _context.Departments
                .FirstOrDefaultAsync(d => d.Id == departmentId && d.CompanyId == companyId && !d.IsDeleted);

            if (department == null)
                throw new InvalidOperationException("Departamento no encontrado");

            var oldValues = new
            {
                department.Name,
                department.BranchId,
                department.Description,
                department.ManagerId,
                department.ParentDepartmentId,
                department.Budget,
                department.Code,
                department.IsActive
            };

            department.Name = request.Name;
            department.BranchId = request.BranchId;
            department.Description = request.Description;
            department.ManagerId = request.ManagerId;
            department.ParentDepartmentId = request.ParentDepartmentId;
            department.Budget = request.Budget;
            department.Code = request.Code;
            if (request.IsActive.HasValue)
                department.IsActive = request.IsActive.Value;
            department.UpdatedAt = DateTime.UtcNow;
            department.UpdatedBy = userId;

            await _context.SaveChangesAsync();

            await LogAuditActionAsync(companyId, userId, "UPDATE", "Department", department.Id, 
                department.Name, oldValues, department, "", "");
        }

        public async System.Threading.Tasks.Task DeleteDepartmentAsync(Guid companyId, Guid departmentId, Guid userId)
        {
            var department = await _context.Departments
                .FirstOrDefaultAsync(d => d.Id == departmentId && d.CompanyId == companyId && !d.IsDeleted);

            if (department == null)
                throw new InvalidOperationException("Departamento no encontrado");

            department.IsDeleted = true;
            department.UpdatedAt = DateTime.UtcNow;
            department.UpdatedBy = userId;

            await _context.SaveChangesAsync();

            await LogAuditActionAsync(companyId, userId, "DELETE", "Department", department.Id, 
                department.Name, department, null, "", "");
        }

        public async System.Threading.Tasks.Task<IEnumerable<DepartmentDto>> GetDepartmentHierarchyAsync(Guid companyId)
        {
            var departments = await _context.Departments
                .Where(d => d.CompanyId == companyId && !d.IsDeleted)
                .Include(d => d.Manager)
                .Include(d => d.Branch)
                .Include(d => d.ParentDepartment)
                .Include(d => d.SubDepartments.Where(sd => !sd.IsDeleted))
                .OrderBy(d => d.Name)
                .ToListAsync();

            return departments.Select(MapToDepartmentDto);
        }
        #endregion

        #region Company Settings
        public async System.Threading.Tasks.Task<CompanySettingsDto?> GetCompanySettingsAsync(Guid companyId)
        {
            var settings = await _context.CompanySettings
                .FirstOrDefaultAsync(s => s.CompanyId == companyId);

            return settings != null ? MapToCompanySettingsDto(settings) : null;
        }

        public async System.Threading.Tasks.Task UpdateCompanySettingsAsync(Guid companyId, UpdateCompanySettingsRequest request, Guid userId)
        {
            var settings = await _context.CompanySettings
                .FirstOrDefaultAsync(s => s.CompanyId == companyId);

            if (settings == null)
            {
                settings = new CompanySettings
                {
                    Id = Guid.NewGuid(),
                    CompanyId = companyId,
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = userId
                };
                _context.CompanySettings.Add(settings);
            }

            var oldValues = settings.Id != Guid.Empty ? new
            {
                settings.Timezone,
                settings.Currency,
                settings.Language,
                settings.DateFormat,
                settings.TimeFormat,
                settings.NumberFormat,
                settings.FiscalYearStartMonth,
                settings.WorkingDays,
                settings.WorkingHoursStart,
                settings.WorkingHoursEnd,
                settings.NotificationSettings,
                settings.SecuritySettings,
                settings.IntegrationSettings,
                settings.CustomSettings
            } : null;

            settings.Timezone = request.Timezone;
            settings.Currency = request.Currency;
            settings.Language = request.Language;
            settings.DateFormat = request.DateFormat;
            settings.TimeFormat = request.TimeFormat;
            settings.NumberFormat = request.NumberFormat;
            if (request.FiscalYearStartMonth.HasValue)
                settings.FiscalYearStartMonth = request.FiscalYearStartMonth.Value;
            settings.WorkingDays = request.WorkingDays;
            if (request.WorkingHoursStart.HasValue)
                settings.WorkingHoursStart = request.WorkingHoursStart.Value;
            if (request.WorkingHoursEnd.HasValue)
                settings.WorkingHoursEnd = request.WorkingHoursEnd.Value;
            settings.NotificationSettings = request.NotificationSettings;
            settings.SecuritySettings = request.SecuritySettings;
            settings.IntegrationSettings = request.IntegrationSettings;
            settings.CustomSettings = request.CustomSettings;
            settings.UpdatedAt = DateTime.UtcNow;
            settings.UpdatedBy = userId;

            await _context.SaveChangesAsync();

            await LogAuditActionAsync(companyId, userId, oldValues == null ? "CREATE" : "UPDATE", 
                "CompanySettings", settings.Id, "Company Settings", oldValues, settings, "", "");
        }

        public async System.Threading.Tasks.Task InitializeDefaultSettingsAsync(Guid companyId, Guid userId)
        {
            var existingSettings = await _context.CompanySettings
                .FirstOrDefaultAsync(s => s.CompanyId == companyId);

            if (existingSettings != null)
                return;

            var defaultSettings = new CompanySettings
            {
                Id = Guid.NewGuid(),
                CompanyId = companyId,
                Timezone = "UTC",
                Currency = "USD",
                Language = "es",
                DateFormat = "dd/MM/yyyy",
                TimeFormat = "HH:mm",
                NumberFormat = "#,##0.00",
                FiscalYearStartMonth = 1,
                WorkingDays = "Monday,Tuesday,Wednesday,Thursday,Friday",
                WorkingHoursStart = TimeSpan.FromHours(8),
                WorkingHoursEnd = TimeSpan.FromHours(17),
                NotificationSettings = JsonSerializer.Serialize(new { emailEnabled = true, smsEnabled = false }),
                SecuritySettings = JsonSerializer.Serialize(new { passwordExpiry = 90, sessionTimeout = 30 }),
                IntegrationSettings = JsonSerializer.Serialize(new { }),
                CustomSettings = JsonSerializer.Serialize(new { }),
                CreatedAt = DateTime.UtcNow,
                CreatedBy = userId
            };

            _context.CompanySettings.Add(defaultSettings);
            await _context.SaveChangesAsync();
        }
        #endregion

        #region Document Management
        public async System.Threading.Tasks.Task<IEnumerable<CompanyDocumentDto>> GetCompanyDocumentsAsync(Guid companyId)
        {
            var documents = await _context.CompanyDocuments
                .Where(d => d.CompanyId == companyId && !d.IsDeleted)
                .Include(d => d.UploadedByUser)
                .Include(d => d.ParentDocument)
                .OrderByDescending(d => d.CreatedAt)
                .ToListAsync();

            return documents.Select(MapToCompanyDocumentDto);
        }

        public async System.Threading.Tasks.Task<CompanyDocumentDto?> GetCompanyDocumentByIdAsync(Guid companyId, Guid documentId)
        {
            var document = await _context.CompanyDocuments
                .Where(d => d.Id == documentId && d.CompanyId == companyId && !d.IsDeleted)
                .Include(d => d.UploadedByUser)
                .Include(d => d.ParentDocument)
                .FirstOrDefaultAsync();

            return document != null ? MapToCompanyDocumentDto(document) : null;
        }

        public async System.Threading.Tasks.Task<Guid> UploadCompanyDocumentAsync(Guid companyId, UploadCompanyDocumentRequest request, Guid userId)
        {
            // Crear directorio si no existe
            var companyDir = Path.Combine(_documentsPath, companyId.ToString());
            Directory.CreateDirectory(companyDir);

            // Generar nombre único para el archivo
            var fileExtension = Path.GetExtension(request.File.FileName);
            var uniqueFileName = $"{Guid.NewGuid()}{fileExtension}";
            var filePath = Path.Combine(companyDir, uniqueFileName);

            // Guardar archivo
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await request.File.CopyToAsync(stream);
            }

            // Calcular hash del archivo
            var fileHash = await CalculateFileHashAsync(filePath);

            var document = new CompanyDocument
            {
                Id = Guid.NewGuid(),
                CompanyId = companyId,
                Name = request.Name,
                OriginalFileName = request.File.FileName,
                Type = request.Type,
                FilePath = filePath,
                MimeType = request.File.ContentType,
                Size = request.File.Length,
                FileHash = fileHash,
                Metadata = request.Metadata,
                ParentDocumentId = request.ParentDocumentId,
                ExpiresAt = request.ExpirationDate,
                CreatedAt = DateTime.UtcNow,
                UploadedBy = userId
            };

            _context.CompanyDocuments.Add(document);
            await _context.SaveChangesAsync();

            await LogAuditActionAsync(companyId, userId, "UPLOAD", "CompanyDocument", document.Id, 
                document.Name, null, document, "", "");

            return document.Id;
        }

        public async System.Threading.Tasks.Task DeleteCompanyDocumentAsync(Guid companyId, Guid documentId, Guid userId)
        {
            var document = await _context.CompanyDocuments
                .FirstOrDefaultAsync(d => d.Id == documentId && d.CompanyId == companyId && !d.IsDeleted);

            if (document == null)
                throw new InvalidOperationException("Documento no encontrado");

            document.IsDeleted = true;
            document.UpdatedAt = DateTime.UtcNow;
            document.UpdatedBy = userId;

            await _context.SaveChangesAsync();

            await LogAuditActionAsync(companyId, userId, "DELETE", "CompanyDocument", document.Id, 
                document.Name, document, null, "", "");
        }

        public async System.Threading.Tasks.Task<(byte[] content, string fileName, string mimeType)> DownloadCompanyDocumentAsync(Guid companyId, Guid documentId)
        {
            var document = await _context.CompanyDocuments
                .FirstOrDefaultAsync(d => d.Id == documentId && d.CompanyId == companyId && !d.IsDeleted);

            if (document == null)
                throw new InvalidOperationException("Documento no encontrado");

            if (!File.Exists(document.FilePath))
                throw new InvalidOperationException("Archivo no encontrado en el sistema");

            var content = await File.ReadAllBytesAsync(document.FilePath);
            return (content, document.OriginalFileName, document.MimeType ?? "application/octet-stream");
        }
        #endregion

        #region Audit and Logging
        public async System.Threading.Tasks.Task<IEnumerable<AuditLog>> GetCompanyAuditLogAsync(Guid companyId, DateTime? fromDate = null, DateTime? toDate = null)
        {
            var query = _context.AuditLogs
                .Where(a => a.CompanyId == companyId);

            if (fromDate.HasValue)
                query = query.Where(a => a.CreatedAt >= fromDate.Value);

            if (toDate.HasValue)
                query = query.Where(a => a.CreatedAt <= toDate.Value);

            return await query
                .Include(a => a.User)
                .OrderByDescending(a => a.CreatedAt)
                .ToListAsync();
        }

        public async System.Threading.Tasks.Task LogAuditActionAsync(Guid companyId, Guid? userId, string action, string entityType, 
            Guid? entityId, string? entityName, object? oldValues, object? newValues, string ipAddress, string? userAgent)
        {
            var auditLog = new AuditLog
            {
                Id = Guid.NewGuid(),
                CompanyId = companyId,
                UserId = userId,
                Action = action,
                EntityType = entityType,
                EntityId = entityId,
                EntityName = entityName,
                OldValues = oldValues != null ? JsonSerializer.Serialize(oldValues) : null,
                NewValues = newValues != null ? JsonSerializer.Serialize(newValues) : null,
                IpAddress = ipAddress,
                UserAgent = userAgent,
                Level = AuditLogLevel.Info,
                CreatedAt = DateTime.UtcNow
            };

            _context.AuditLogs.Add(auditLog);
            await _context.SaveChangesAsync();
        }
        #endregion

        #region Private Methods
        private static BranchDto MapToBranchDto(Branch branch)
        {
            return new BranchDto
            {
                Id = branch.Id,
                CompanyId = branch.CompanyId,
                Name = branch.Name,
                Address = branch.Address,
                City = branch.City,
                State = branch.State,
                Country = branch.Country,
                PostalCode = branch.PostalCode,
                Phone = branch.Phone,
                Email = branch.Email,
                ManagerId = branch.ManagerId,
                ManagerName = branch.Manager?.FirstName + " " + branch.Manager?.LastName,
                IsActive = branch.IsActive,
                CreatedAt = branch.CreatedAt,
                UpdatedAt = branch.UpdatedAt,
                CreatedBy = branch.CreatedBy,
                UpdatedBy = branch.UpdatedBy
            };
        }

        private static DepartmentDto MapToDepartmentDto(Department department)
        {
            return new DepartmentDto
            {
                Id = department.Id,
                CompanyId = department.CompanyId,
                Name = department.Name,
                BranchId = department.BranchId,
                BranchName = department.Branch?.Name,
                Description = department.Description,
                ManagerId = department.ManagerId,
                ManagerName = department.Manager?.FirstName + " " + department.Manager?.LastName,
                ParentDepartmentId = department.ParentDepartmentId,
                ParentDepartmentName = department.ParentDepartment?.Name,
                Budget = department.Budget,
                Code = department.Code,
                IsActive = department.IsActive,
                IsDeleted = department.IsDeleted,
                CreatedAt = department.CreatedAt,
                UpdatedAt = department.UpdatedAt,
                CreatedBy = department.CreatedBy,
                UpdatedBy = department.UpdatedBy,
                SubDepartments = department.SubDepartments?.Where(sd => !sd.IsDeleted).Select(sd => new DepartmentDto
                {
                    Id = sd.Id,
                    Name = sd.Name,
                    Code = sd.Code,
                    IsActive = sd.IsActive
                }).ToList() ?? new List<DepartmentDto>(),
                EmployeeCount = 0 // TODO: Implementar cuando se tenga la entidad Employee
            };
        }

        private static CompanySettingsDto MapToCompanySettingsDto(CompanySettings settings)
        {
            return new CompanySettingsDto
            {
                Id = settings.Id,
                CompanyId = settings.CompanyId,
                Timezone = settings.Timezone,
                Currency = settings.Currency,
                Language = settings.Language,
                DateFormat = settings.DateFormat,
                TimeFormat = settings.TimeFormat,
                NumberFormat = settings.NumberFormat,
                FiscalYearStartMonth = settings.FiscalYearStartMonth,
                WorkingDays = settings.WorkingDays,
                WorkingHoursStart = settings.WorkingHoursStart,
                WorkingHoursEnd = settings.WorkingHoursEnd,
                NotificationSettings = settings.NotificationSettings,
                SecuritySettings = settings.SecuritySettings,
                IntegrationSettings = settings.IntegrationSettings,
                CustomSettings = settings.CustomSettings,
                CreatedAt = settings.CreatedAt,
                UpdatedAt = settings.UpdatedAt,
                CreatedBy = settings.CreatedBy,
                UpdatedBy = settings.UpdatedBy
            };
        }

        private static CompanyDocumentDto MapToCompanyDocumentDto(CompanyDocument document)
        {
            return new CompanyDocumentDto
            {
                Id = document.Id,
                CompanyId = document.CompanyId,
                Name = document.Name,
                OriginalFileName = document.OriginalFileName,
                Type = document.Type,
                FilePath = document.FilePath,
                MimeType = document.MimeType,
                Size = document.Size,
                FileHash = document.FileHash,
                Metadata = document.Metadata,
                Version = document.Version,
                ParentDocumentId = document.ParentDocumentId,
                ParentDocumentName = document.ParentDocument?.Name,
                ExpirationDate = document.ExpiresAt,
                IsDeleted = document.IsDeleted,
                CreatedAt = document.CreatedAt,
                UpdatedAt = document.UpdatedAt,
                CreatedBy = document.UploadedBy,
                CreatedByName = document.UploadedByUser?.FirstName + " " + document.UploadedByUser?.LastName,
                UpdatedBy = document.UpdatedBy
            };
        }

        private static async System.Threading.Tasks.Task<string> CalculateFileHashAsync(string filePath)
        {
            using var sha256 = System.Security.Cryptography.SHA256.Create();
            using var stream = File.OpenRead(filePath);
            var hash = await sha256.ComputeHashAsync(stream);
            return Convert.ToBase64String(hash);
        }
        #endregion
    }
}