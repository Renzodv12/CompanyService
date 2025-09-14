using CompanyService.Core.DTOs.DynamicReports;
using CompanyService.Core.Entities;
using CompanyService.Core.Enums;
using CompanyService.Core.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Text;
using System.Text.Json;
using System.Linq.Dynamic.Core;
using CompanyService.Infrastructure.Context;
using Task = System.Threading.Tasks.Task;
using ExportFormatEnum = CompanyService.Core.Enums.ExportFormat;
// using ExportFormatDto = CompanyService.Core.DTOs.DynamicReports.ExportFormat; // Removed - using enum directly

namespace CompanyService.Infrastructure.Services
{
    public class DynamicReportService : IDynamicReportService
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<DynamicReportService> _logger;
        private readonly Dictionary<string, Type> _allowedEntities;

        public DynamicReportService(ApplicationDbContext context, ILogger<DynamicReportService> logger)
        {
            _context = context;
            _logger = logger;
            _allowedEntities = InitializeAllowedEntities();
        }

        private Dictionary<string, Type> InitializeAllowedEntities()
        {
            return new Dictionary<string, Type>
            {
                { "Product", typeof(Product) },
                { "Sale", typeof(Sale) },
                { "Customer", typeof(Customer) },
                { "Purchase", typeof(Purchase) },
                { "Supplier", typeof(Supplier) },
                { "Account", typeof(Account) },
                { "JournalEntry", typeof(JournalEntry) },
                { "StockMovement", typeof(StockMovement) },
                { "Task", typeof(Core.Entities.Task) },
                { "Event", typeof(Event) }
            };
        }

        public async Task<ReportDefinitionResponseDto> CreateReportDefinitionAsync(CreateReportDefinitionDto dto, Guid userId)
        {
            // Validate entity name
            if (!_allowedEntities.ContainsKey(dto.EntityName))
            {
                throw new ArgumentException($"Entity '{dto.EntityName}' is not allowed for reporting.");
            }

            // Validate user has access to company
            var userCompany = await _context.UserCompanys
                .FirstOrDefaultAsync(uc => uc.UserId == userId && uc.CompanyId == dto.CompanyId);
            
            if (userCompany == null)
            {
                throw new UnauthorizedAccessException("User does not have access to this company.");
            }

            // Validate fields and filters
            await ValidateFieldsAndFiltersAsync(dto.Fields.Cast<object>().Concat(dto.Filters.Cast<object>()), dto.EntityName);

            var reportDefinition = new ReportDefinition
            {
                Name = dto.Name,
                Description = dto.Description,
                EntityName = dto.EntityName,
                CompanyId = dto.CompanyId,
                IsShared = dto.IsShared,
                CreatedByUserId = userId
            };

            _context.ReportDefinitions.Add(reportDefinition);
            await _context.SaveChangesAsync();

            // Add fields
            foreach (var fieldDto in dto.Fields)
            {
                var field = new ReportField
                {
                    ReportDefinitionId = reportDefinition.Id,
                    FieldName = fieldDto.FieldName,
                    DisplayName = fieldDto.DisplayName,
                    DataType = fieldDto.DataType.ToString(),
                    DisplayOrder = fieldDto.DisplayOrder,
                    IsVisible = fieldDto.IsVisible,
                    AggregateFunction = fieldDto.AggregateFunction?.ToString(),
                    FormatString = fieldDto.FormatString
                };
                _context.ReportFields.Add(field);
            }

            // Add filters
            foreach (var filterDto in dto.Filters)
            {
                var filter = new ReportFilter
                {
                    ReportDefinitionId = reportDefinition.Id,
                    FieldName = filterDto.FieldName,
                    DisplayName = filterDto.DisplayName,
                    DataType = filterDto.DataType.ToString(),
                    Operator = filterDto.Operator.ToString(),
                    DefaultValue = filterDto.DefaultValue,
                    IsRequired = filterDto.IsRequired,
                    IsUserEditable = filterDto.IsUserEditable,
                    DisplayOrder = filterDto.DisplayOrder
                };
                _context.ReportFilters.Add(filter);
            }

            await _context.SaveChangesAsync();

            return await GetReportDefinitionAsync(reportDefinition.Id, dto.CompanyId) 
                ?? throw new InvalidOperationException("Failed to retrieve created report definition.");
        }

        public async Task<ReportDefinitionResponseDto> UpdateReportDefinitionAsync(Guid id, UpdateReportDefinitionDto dto, Guid userId, Guid companyId)
        {
            var reportDefinition = await _context.ReportDefinitions
                .Include(rd => rd.Fields)
                .Include(rd => rd.Filters)
                .FirstOrDefaultAsync(rd => rd.Id == id && rd.CompanyId == companyId);

            if (reportDefinition == null)
            {
                throw new ArgumentException("Report definition not found.");
            }

            // Check if user can modify this report
            if (reportDefinition.CreatedByUserId != userId && !reportDefinition.IsShared)
            {
                throw new UnauthorizedAccessException("User cannot modify this report definition.");
            }

            // Update basic properties
            reportDefinition.Name = dto.Name;
            reportDefinition.Description = dto.Description;
            reportDefinition.IsActive = dto.IsActive;
            reportDefinition.IsShared = dto.IsShared;
            reportDefinition.LastModifiedAt = DateTime.UtcNow;
            reportDefinition.Version++;

            // Update fields (simple approach: remove all and re-add)
            _context.ReportFields.RemoveRange(reportDefinition.Fields);
            foreach (var fieldDto in dto.Fields)
            {
                var field = new ReportField
                {
                    ReportDefinitionId = reportDefinition.Id,
                    FieldName = fieldDto.FieldName,
                    DisplayName = fieldDto.DisplayName,
                    DataType = fieldDto.DataType.ToString(),
                    DisplayOrder = fieldDto.DisplayOrder,
                    IsVisible = fieldDto.IsVisible,
                    AggregateFunction = fieldDto.AggregateFunction?.ToString(),
                    FormatString = fieldDto.FormatString
                };
                _context.ReportFields.Add(field);
            }

            // Update filters
            _context.ReportFilters.RemoveRange(reportDefinition.Filters);
            foreach (var filterDto in dto.Filters)
            {
                var filter = new ReportFilter
                {
                    ReportDefinitionId = reportDefinition.Id,
                    FieldName = filterDto.FieldName,
                    DisplayName = filterDto.DisplayName,
                    DataType = filterDto.DataType.ToString(),
                    Operator = filterDto.Operator.ToString(),
                    DefaultValue = filterDto.DefaultValue,
                    IsRequired = filterDto.IsRequired,
                    IsUserEditable = filterDto.IsUserEditable,
                    DisplayOrder = filterDto.DisplayOrder
                };
                _context.ReportFilters.Add(filter);
            }

            await _context.SaveChangesAsync();

            return await GetReportDefinitionAsync(id, companyId) 
                ?? throw new InvalidOperationException("Failed to retrieve updated report definition.");
        }

        public async Task<bool> DeleteReportDefinitionAsync(Guid id, Guid userId, Guid companyId)
        {
            var reportDefinition = await _context.ReportDefinitions
                .FirstOrDefaultAsync(rd => rd.Id == id && rd.CompanyId == companyId);

            if (reportDefinition == null)
            {
                return false;
            }

            // Check if user can delete this report
            if (reportDefinition.CreatedByUserId != userId)
            {
                throw new UnauthorizedAccessException("User cannot delete this report definition.");
            }

            _context.ReportDefinitions.Remove(reportDefinition);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<ReportDefinitionResponseDto?> GetReportDefinitionAsync(Guid id, Guid companyId)
        {
            var reportDefinition = await _context.ReportDefinitions
                .Include(rd => rd.Company)
                .Include(rd => rd.Fields.OrderBy(rf => rf.DisplayOrder))
                .Include(rd => rd.Filters.OrderBy(rf => rf.DisplayOrder))
                .FirstOrDefaultAsync(rd => rd.Id == id && rd.CompanyId == companyId);

            if (reportDefinition == null)
            {
                return null;
            }

            return new ReportDefinitionResponseDto
            {
                Id = reportDefinition.Id,
                Name = reportDefinition.Name,
                Description = reportDefinition.Description,
                EntityName = reportDefinition.EntityName,
                CompanyId = reportDefinition.CompanyId,
                CompanyName = reportDefinition.Company?.Name ?? "",
                IsActive = reportDefinition.IsActive,
                IsShared = reportDefinition.IsShared,
                Version = reportDefinition.Version,
                CreatedAt = reportDefinition.CreatedAt,
                LastModifiedAt = reportDefinition.LastModifiedAt,
                CreatedByUserId = reportDefinition.CreatedByUserId,
                CreatedByUserName = "", // TODO: Get from user service
                Fields = reportDefinition.Fields.Select(rf => new ReportFieldResponseDto
                {
                    Id = rf.Id,
                    FieldName = rf.FieldName,
                    DisplayName = rf.DisplayName,
                    DataType = Enum.TryParse<ReportDataType>(rf.DataType, out var dataType) ? dataType : ReportDataType.String,
                    DisplayOrder = rf.DisplayOrder,
                    IsVisible = rf.IsVisible,
                    AggregateFunction = string.IsNullOrEmpty(rf.AggregateFunction) ? null : Enum.TryParse<AggregateFunction>(rf.AggregateFunction, out var aggFunc) ? aggFunc : null,
                    FormatString = rf.FormatString,
                    CreatedAt = rf.CreatedAt
                }).ToList(),
                Filters = reportDefinition.Filters.Select(rf => new ReportFilterResponseDto
                {
                    Id = rf.Id,
                    FieldName = rf.FieldName,
                    DisplayName = rf.DisplayName,
                    DataType = Enum.TryParse<ReportDataType>(rf.DataType, out var filterDataType) ? filterDataType : ReportDataType.String,
                    Operator = Enum.TryParse<FilterOperator>(rf.Operator, out var filterOp) ? filterOp : FilterOperator.Equals,
                    DefaultValue = rf.DefaultValue,
                    IsRequired = rf.IsRequired,
                    IsUserEditable = rf.IsUserEditable,
                    DisplayOrder = rf.DisplayOrder,
                    CreatedAt = rf.CreatedAt
                }).ToList()
            };
        }

        public async Task<IEnumerable<ReportDefinitionListDto>> GetReportDefinitionsAsync(Guid companyId, bool includeShared = true)
        {
            var query = _context.ReportDefinitions
                .Where(rd => rd.CompanyId == companyId && rd.IsActive);

            if (includeShared)
            {
                query = query.Where(rd => rd.IsShared || rd.CompanyId == companyId);
            }

            var reportDefinitions = await query
                .Include(rd => rd.Fields)
                .Include(rd => rd.Filters)
                .OrderByDescending(rd => rd.CreatedAt)
                .ToListAsync();

            return reportDefinitions.Select(rd => new ReportDefinitionListDto
            {
                Id = rd.Id,
                Name = rd.Name,
                Description = rd.Description,
                EntityName = rd.EntityName,
                IsActive = rd.IsActive,
                IsShared = rd.IsShared,
                Version = rd.Version,
                CreatedAt = rd.CreatedAt,
                LastModifiedAt = rd.LastModifiedAt,
                CreatedByUserName = "", // TODO: Get from user service
                FieldsCount = rd.Fields.Count,
                FiltersCount = rd.Filters.Count
            });
        }

        public async Task<ReportExecutionResponseDto> ExecuteReportAsync(ExecuteReportDto dto, Guid userId)
        {
            // Validate access
            if (!await CanUserExecuteReportAsync(dto.ReportDefinitionId, userId, dto.CompanyId))
            {
                throw new UnauthorizedAccessException("User cannot execute this report.");
            }

            var reportDefinition = await _context.ReportDefinitions
                .Include(rd => rd.Fields)
                .Include(rd => rd.Filters)
                .FirstOrDefaultAsync(rd => rd.Id == dto.ReportDefinitionId && rd.CompanyId == dto.CompanyId);

            if (reportDefinition == null)
            {
                throw new ArgumentException("Report definition not found.");
            }

            // Create execution record
            var execution = new DynamicReportExecution
            {
                ReportDefinitionId = dto.ReportDefinitionId,
                ExecutedByUserId = userId,
                CompanyId = dto.CompanyId,
                Status = ReportExecutionStatus.Running,
                FiltersApplied = JsonSerializer.Serialize(dto.FilterValues),
                ExportFormat = dto.ExportFormat?.ToString(),
                ExecutedAt = DateTime.UtcNow,
                ExpiresAt = DateTime.UtcNow.AddDays(7) // Reports expire after 7 days
            };

            _context.DynamicReportExecutions.Add(execution);
            await _context.SaveChangesAsync();

            try
            {
                var startTime = DateTime.UtcNow;
                
                // Execute the dynamic query
                var data = await ExecuteDynamicQueryAsync(reportDefinition, dto.FilterValues, dto.PageNumber ?? 1, dto.PageSize ?? 50);
                
                var endTime = DateTime.UtcNow;
                execution.ExecutionDuration = endTime - startTime;
                execution.RowCount = data.TotalRows;
                execution.Status = ReportExecutionStatus.Completed;

                await _context.SaveChangesAsync();

                return new ReportExecutionResponseDto
                {
                    Id = execution.Id,
                    ReportDefinitionId = execution.ReportDefinitionId,
                    ReportDefinitionName = reportDefinition.Name,
                    ExecutedByUserId = execution.ExecutedByUserId,
                    ExecutedByUserName = "", // TODO: Get from user service
                    CompanyId = execution.CompanyId,
                    Status = execution.Status,
                    FiltersApplied = execution.FiltersApplied,
                    RowCount = execution.RowCount,
                    ExecutionDuration = execution.ExecutionDuration,
                    ExportFormat = Enum.TryParse<ExportFormat>(execution.ExportFormat, out var format) ? format : null,
                    ExecutedAt = execution.ExecutedAt,
                    ExpiresAt = execution.ExpiresAt,
                    Data = data.Data,
                    Metadata = data.Metadata
                };
            }
            catch (Exception ex)
            {
                execution.Status = ReportExecutionStatus.Failed;
                execution.ErrorMessage = ex.Message;
                await _context.SaveChangesAsync();
                throw;
            }
        }

        private async Task<(List<Dictionary<string, object?>> Data, ReportMetadataDto Metadata, int TotalRows)> ExecuteDynamicQueryAsync(
            ReportDefinition reportDefinition, 
            List<ReportFilterValueDto> filterValues, 
            int pageNumber, 
            int pageSize)
        {
            // This is a simplified implementation. In a real scenario, you would need to:
            // 1. Build dynamic LINQ queries based on the entity type
            // 2. Apply filters securely
            // 3. Apply aggregations
            // 4. Handle pagination
            // 5. Ensure proper SQL injection protection

            // For now, return mock data
            var mockData = new List<Dictionary<string, object?>>
            {
                new() { ["Id"] = Guid.NewGuid(), ["Name"] = "Sample Data 1", ["Value"] = 100 },
                new() { ["Id"] = Guid.NewGuid(), ["Name"] = "Sample Data 2", ["Value"] = 200 }
            };

            var metadata = new ReportMetadataDto
            {
                TotalRows = 2,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalPages = 1,
                HasNextPage = false,
                HasPreviousPage = false,
                Columns = reportDefinition.Fields.Select(rf => new ReportColumnMetadataDto
                {
                    FieldName = rf.FieldName,
                    DisplayName = rf.DisplayName,
                    DataType = Enum.TryParse<ReportDataType>(rf.DataType, out var colDataType) ? colDataType : ReportDataType.String,
                    AggregateFunction = string.IsNullOrEmpty(rf.AggregateFunction) ? null : Enum.TryParse<AggregateFunction>(rf.AggregateFunction, out var colAggFunc) ? colAggFunc : null,
                    FormatString = rf.FormatString,
                    DisplayOrder = rf.DisplayOrder
                }).ToList()
            };

            return (mockData, metadata, 2);
        }

        public async Task<ReportExecutionResponseDto?> GetReportExecutionAsync(Guid executionId, Guid companyId)
        {
            var execution = await _context.DynamicReportExecutions
                .Include(e => e.ReportDefinition)
                .FirstOrDefaultAsync(e => e.Id == executionId && e.CompanyId == companyId);

            if (execution == null)
            {
                return null;
            }

            return new ReportExecutionResponseDto
            {
                Id = execution.Id,
                ReportDefinitionId = execution.ReportDefinitionId,
                ReportDefinitionName = execution.ReportDefinition?.Name ?? "",
                ExecutedByUserId = execution.ExecutedByUserId,
                ExecutedByUserName = "", // TODO: Get from user service
                CompanyId = execution.CompanyId,
                Status = execution.Status,
                FiltersApplied = execution.FiltersApplied,
                RowCount = execution.RowCount,
                ExecutionDuration = execution.ExecutionDuration,
                ErrorMessage = execution.ErrorMessage,
                ExportFormat = Enum.TryParse<ExportFormat>(execution.ExportFormat, out var execFormat) ? execFormat : null,
                ExportFileName = execution.ExportFileName,
                FileSizeBytes = execution.FileSizeBytes,
                ExecutedAt = execution.ExecutedAt,
                ExpiresAt = execution.ExpiresAt
            };
        }

        public async Task<IEnumerable<ReportExecutionListDto>> GetReportExecutionsAsync(Guid companyId, Guid? userId = null, int pageNumber = 1, int pageSize = 50)
        {
            var query = _context.DynamicReportExecutions
                .Include(e => e.ReportDefinition)
                .Where(e => e.CompanyId == companyId);

            if (userId.HasValue)
            {
                query = query.Where(e => e.ExecutedByUserId == userId.Value);
            }

            var executions = await query
                .OrderByDescending(e => e.ExecutedAt)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return executions.Select(e => new ReportExecutionListDto
            {
                Id = e.Id,
                ReportDefinitionName = e.ReportDefinition?.Name ?? "",
                ExecutedByUserName = "", // TODO: Get from user service
                Status = e.Status,
                RowCount = e.RowCount,
                ExecutionDuration = e.ExecutionDuration,
                ExportFormat = Enum.TryParse<ExportFormat>(e.ExportFormat, out var listFormat) ? listFormat : null,
                ExportFileName = e.ExportFileName,
                FileSizeBytes = e.FileSizeBytes,
                ExecutedAt = e.ExecutedAt,
                ExpiresAt = e.ExpiresAt
            });
        }

        public async Task<byte[]> ExportReportAsync(Guid executionId, ExportFormat format, Guid companyId)
        {
            var execution = await _context.DynamicReportExecutions
                .Include(re => re.ReportDefinition)
                .FirstOrDefaultAsync(re => re.Id == executionId && re.CompanyId == companyId);

            if (execution == null)
            {
                throw new ArgumentException("Report execution not found.");
            }

            if (execution.Status != ReportExecutionStatus.Completed)
            {
                throw new InvalidOperationException("Report execution is not completed.");
            }

            // Re-execute the report to get fresh data for export
            var reportData = await ExecuteDynamicQueryAsync(
                execution.ReportDefinition, 
                new List<ReportFilterValueDto>(), // For export, use empty filters or deserialize properly
                1, 
                int.MaxValue // Get all data for export
            );

            // Use ReportExportService if available, otherwise implement basic export
            return format switch
            {
                ExportFormat.Json => System.Text.Encoding.UTF8.GetBytes(JsonSerializer.Serialize(reportData.Data, new JsonSerializerOptions { WriteIndented = true })),
                ExportFormat.Csv => ExportToCsv(reportData.Data),
                _ => throw new NotSupportedException($"Export format {format} is not yet supported.")
            };
        }

        private byte[] ExportToCsv(List<Dictionary<string, object?>> data)
        {
            if (!data.Any())
                return System.Text.Encoding.UTF8.GetBytes(string.Empty);

            var csv = new StringBuilder();
            
            // Headers
            var headers = data.First().Keys;
            csv.AppendLine(string.Join(",", headers.Select(h => $"\"{h}\"")));
            
            // Data rows
            foreach (var row in data)
            {
                var values = headers.Select(h => 
                {
                    var value = row.ContainsKey(h) ? row[h]?.ToString() ?? string.Empty : string.Empty;
                    return $"\"{value.Replace("\"", "\"\"")}\"";
                });
                csv.AppendLine(string.Join(",", values));
            }
            
            return System.Text.Encoding.UTF8.GetBytes(csv.ToString());
        }

        public async Task<bool> ValidateReportDefinitionAsync(CreateReportDefinitionDto dto, Guid companyId)
        {
            // Validate entity exists and is allowed
            if (!_allowedEntities.ContainsKey(dto.EntityName))
            {
                return false;
            }

            // Validate fields and filters
            try
            {
                await ValidateFieldsAndFiltersAsync(dto.Fields.Cast<object>().Concat(dto.Filters.Cast<object>()), dto.EntityName);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> CanUserAccessReportAsync(Guid reportDefinitionId, Guid userId, Guid companyId)
        {
            var userCompany = await _context.UserCompanys
                .FirstOrDefaultAsync(uc => uc.UserId == userId && uc.CompanyId == companyId);

            if (userCompany == null)
            {
                return false;
            }

            var reportDefinition = await _context.ReportDefinitions
                .FirstOrDefaultAsync(rd => rd.Id == reportDefinitionId && 
                                          (rd.CompanyId == companyId || rd.IsShared));

            return reportDefinition != null;
        }

        public async Task<bool> CanUserExecuteReportAsync(Guid reportDefinitionId, Guid userId, Guid companyId)
        {
            return await CanUserAccessReportAsync(reportDefinitionId, userId, companyId);
        }

        public async Task<IEnumerable<string>> GetAvailableEntitiesAsync(Guid companyId)
        {
            // Return allowed entities for the company
            return _allowedEntities.Keys;
        }

        public async Task<IEnumerable<EntityFieldDto>> GetEntityFieldsAsync(string entityName, Guid companyId)
        {
            if (!_allowedEntities.TryGetValue(entityName, out var entityType))
            {
                throw new ArgumentException($"Entity '{entityName}' is not allowed.");
            }

            // Use reflection to get entity properties
            var properties = entityType.GetProperties()
                .Where(p => p.CanRead && IsAllowedPropertyType(p.PropertyType))
                .Select(p => new EntityFieldDto
                {
                    FieldName = p.Name,
                    DisplayName = p.Name, // TODO: Use display attributes if available
                    DataType = GetDataTypeString(p.PropertyType),
                    IsRequired = !IsNullableType(p.PropertyType),
                    IsFilterable = IsFilterableType(p.PropertyType),
                    IsAggregatable = IsAggregatableType(p.PropertyType),
                    Description = null // TODO: Use description attributes if available
                });

            return properties;
        }

        private async Task ValidateFieldsAndFiltersAsync(IEnumerable<object> fieldsAndFilters, string entityName)
        {
            if (!_allowedEntities.TryGetValue(entityName, out var entityType))
            {
                throw new ArgumentException($"Entity '{entityName}' is not allowed.");
            }

            var allowedFields = entityType.GetProperties().Select(p => p.Name).ToHashSet();

            foreach (var item in fieldsAndFilters)
            {
                string fieldName = item switch
                {
                    CreateReportFieldDto field => field.FieldName,
                    CreateReportFilterDto filter => filter.FieldName,
                    UpdateReportFieldDto updateField => updateField.FieldName,
                    UpdateReportFilterDto updateFilter => updateFilter.FieldName,
                    _ => throw new ArgumentException("Invalid field or filter type.")
                };

                if (!allowedFields.Contains(fieldName))
                {
                    throw new ArgumentException($"Field '{fieldName}' is not valid for entity '{entityName}'.");
                }
            }
        }

        private static bool IsAllowedPropertyType(Type type)
        {
            var underlyingType = Nullable.GetUnderlyingType(type) ?? type;
            return underlyingType.IsPrimitive || 
                   underlyingType == typeof(string) || 
                   underlyingType == typeof(DateTime) || 
                   underlyingType == typeof(decimal) || 
                   underlyingType == typeof(Guid);
        }

        private static string GetDataTypeString(Type type)
        {
            var underlyingType = Nullable.GetUnderlyingType(type) ?? type;
            
            if (underlyingType == typeof(string)) return "String";
            if (underlyingType == typeof(int) || underlyingType == typeof(long)) return "Integer";
            if (underlyingType == typeof(decimal) || underlyingType == typeof(double) || underlyingType == typeof(float)) return "Decimal";
            if (underlyingType == typeof(DateTime)) return "DateTime";
            if (underlyingType == typeof(bool)) return "Boolean";
            if (underlyingType == typeof(Guid)) return "Guid";
            
            return "String";
        }

        private static bool IsNullableType(Type type)
        {
            return !type.IsValueType || Nullable.GetUnderlyingType(type) != null;
        }

        private static bool IsFilterableType(Type type)
        {
            return IsAllowedPropertyType(type);
        }

        private static bool IsAggregatableType(Type type)
        {
            var underlyingType = Nullable.GetUnderlyingType(type) ?? type;
            return underlyingType == typeof(int) || 
                   underlyingType == typeof(long) || 
                   underlyingType == typeof(decimal) || 
                   underlyingType == typeof(double) || 
                   underlyingType == typeof(float);
        }
    }
}