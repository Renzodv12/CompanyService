using CompanyService.Application.Commands.DynamicReports;
using CompanyService.Application.Queries.DynamicReports;
using CompanyService.Core.DTOs.DynamicReports;
using CompanyService.Core.Enums;
using CompanyService.WebApi.Extensions;
using MediatR;

namespace CompanyService.WebApi.Endpoints
{
    public static class DynamicReportsEndpoints
    {
        public static IEndpointRouteBuilder MapDynamicReportsEndpoints(this IEndpointRouteBuilder app)
        {
            // Report Definitions Endpoints
            app.MapGet("/companies/{companyId:guid}/reports/definitions", GetReportDefinitions)
                .WithName("GetReportDefinitions")
                .WithTags("DynamicReports")
                .RequireAuthorization()
                .Produces<IEnumerable<ReportDefinitionListDto>>(StatusCodes.Status200OK)
                .WithOpenApi();

            app.MapGet("/companies/{companyId:guid}/reports/definitions/{id:guid}", GetReportDefinition)
                .WithName("GetReportDefinition")
                .WithTags("DynamicReports")
                .RequireAuthorization()
                .Produces<ReportDefinitionResponseDto>(StatusCodes.Status200OK)
                .WithOpenApi();

            app.MapPost("/companies/{companyId:guid}/reports/definitions", CreateReportDefinition)
                .WithName("CreateReportDefinition")
                .WithTags("DynamicReports")
                .RequireAuthorization()
                .Accepts<CreateReportDefinitionDto>("application/json")
                .Produces<ReportDefinitionResponseDto>(StatusCodes.Status201Created)
                .Produces(StatusCodes.Status400BadRequest)
                .WithOpenApi();

            app.MapPut("/companies/{companyId:guid}/reports/definitions/{id:guid}", UpdateReportDefinition)
                .WithName("UpdateReportDefinition")
                .WithTags("DynamicReports")
                .RequireAuthorization()
                .Accepts<UpdateReportDefinitionDto>("application/json")
                .Produces<ReportDefinitionResponseDto>(StatusCodes.Status200OK)
                .WithOpenApi();

            app.MapDelete("/companies/{companyId:guid}/reports/definitions/{id:guid}", DeleteReportDefinition)
                .WithName("DeleteReportDefinition")
                .WithTags("DynamicReports")
                .RequireAuthorization()
                .Produces(StatusCodes.Status200OK)
                .WithOpenApi();

            // Report Execution Endpoints
            app.MapPost("/companies/{companyId:guid}/reports/execute", ExecuteReport)
                .WithName("ExecuteReport")
                .WithTags("DynamicReports")
                .RequireAuthorization()
                .Accepts<ExecuteReportDto>("application/json")
                .Produces<ReportExecutionResponseDto>(StatusCodes.Status200OK)
                .WithOpenApi();

            app.MapGet("/companies/{companyId:guid}/reports/executions", GetReportExecutions)
                .WithName("GetReportExecutions")
                .WithTags("DynamicReports")
                .RequireAuthorization()
                .Produces<IEnumerable<ReportExecutionListDto>>(StatusCodes.Status200OK)
                .WithOpenApi();

            app.MapGet("/companies/{companyId:guid}/reports/executions/{executionId:guid}/export", ExportReportExecution)
                .WithName("ExportReportExecution")
                .WithTags("DynamicReports")
                .RequireAuthorization()
                .Produces(StatusCodes.Status200OK, contentType: "application/octet-stream")
                .WithOpenApi();

            // Metadata Endpoints
            app.MapGet("/companies/{companyId:guid}/reports/entities", GetAvailableEntities)
                .WithName("GetAvailableEntities")
                .WithTags("DynamicReports")
                .RequireAuthorization()
                .Produces<IEnumerable<EntityMetadataDto>>(StatusCodes.Status200OK)
                .WithOpenApi();

            app.MapGet("/companies/{companyId:guid}/reports/entities/{entityName}/fields", GetEntityFields)
                .WithName("GetEntityFields")
                .WithTags("DynamicReports")
                .RequireAuthorization()
                .Produces<IEnumerable<FieldMetadataDto>>(StatusCodes.Status200OK)
                .WithOpenApi();

            return app;
        }

        private static async Task<IResult> GetReportDefinitions(
            Guid companyId,
            bool includeShared,
            HttpContext httpContext,
            IMediator mediator)
        {
            try
            {
                var claims = httpContext.ExtractTokenClaims();
                var query = new GetReportDefinitionsQuery
                {
                    CompanyId = companyId,
                    UserId = Guid.Parse(claims.UserId!),
                    IncludeShared = includeShared
                };

                var result = await mediator.Send(query);
                return Results.Ok(result);
            }
            catch (Exception ex)
            {
                return Results.Problem(ex.Message);
            }
        }

        private static async Task<IResult> GetReportDefinition(
            Guid companyId,
            Guid id,
            HttpContext httpContext,
            IMediator mediator)
        {
            try
            {
                var claims = httpContext.ExtractTokenClaims();
                var query = new GetReportDefinitionQuery
                {
                    Id = id,
                    CompanyId = companyId,
                    UserId = Guid.Parse(claims.UserId!)
                };

                var result = await mediator.Send(query);
                return Results.Ok(result);
            }
            catch (ArgumentException ex)
            {
                return Results.NoContent();
            }
            catch (UnauthorizedAccessException)
            {
                return Results.Forbid();
            }
            catch (Exception ex)
            {
                return Results.Problem(ex.Message);
            }
        }

        private static async Task<IResult> CreateReportDefinition(
            Guid companyId,
            CreateReportDefinitionDto dto,
            HttpContext httpContext,
            IMediator mediator)
        {
            try
            {
                var claims = httpContext.ExtractTokenClaims();
                var command = new CreateReportDefinitionCommand
                {
                    ReportDefinition = dto,
                    UserId = Guid.Parse(claims.UserId!),
                    CompanyId = companyId
                };

                var result = await mediator.Send(command);
                return Results.Created($"/companies/{companyId}/reports/definitions/{result.Id}", result);
            }
            catch (ArgumentException ex)
            {
                return Results.BadRequest(ex.Message);
            }
            catch (UnauthorizedAccessException)
            {
                return Results.Forbid();
            }
            catch (Exception ex)
            {
                return Results.Problem(ex.Message);
            }
        }

        private static async Task<IResult> UpdateReportDefinition(
            Guid companyId,
            Guid id,
            UpdateReportDefinitionDto dto,
            HttpContext httpContext,
            IMediator mediator)
        {
            try
            {
                var claims = httpContext.ExtractTokenClaims();
                var command = new UpdateReportDefinitionCommand
                {
                    Id = id,
                    ReportDefinition = dto,
                    UserId = Guid.Parse(claims.UserId!),
                    CompanyId = companyId
                };

                var result = await mediator.Send(command);
                return Results.Ok(result);
            }
            catch (ArgumentException ex)
            {
                return Results.BadRequest(ex.Message);
            }
            catch (UnauthorizedAccessException)
            {
                return Results.Forbid();
            }
            catch (Exception ex)
            {
                return Results.Problem(ex.Message);
            }
        }

        private static async Task<IResult> DeleteReportDefinition(
            Guid companyId,
            Guid id,
            HttpContext httpContext,
            IMediator mediator)
        {
            try
            {
                var claims = httpContext.ExtractTokenClaims();
                var command = new DeleteReportDefinitionCommand
                {
                    Id = id,
                    UserId = Guid.Parse(claims.UserId!),
                    CompanyId = companyId
                };

                await mediator.Send(command);
                return Results.Ok();
            }
            catch (ArgumentException ex)
            {
                return Results.NoContent();
            }
            catch (UnauthorizedAccessException)
            {
                return Results.Forbid();
            }
            catch (Exception ex)
            {
                return Results.Problem(ex.Message);
            }
        }

        private static async Task<IResult> ExecuteReport(
            Guid companyId,
            ExecuteReportDto dto,
            HttpContext httpContext,
            IMediator mediator)
        {
            try
            {
                var claims = httpContext.ExtractTokenClaims();
                var command = new ExecuteReportCommand
                {
                    ExecuteRequest = dto,
                    UserId = Guid.Parse(claims.UserId!)
                };

                var result = await mediator.Send(command);
                return Results.Ok(result);
            }
            catch (ArgumentException ex)
            {
                return Results.BadRequest(ex.Message);
            }
            catch (UnauthorizedAccessException)
            {
                return Results.Forbid();
            }
            catch (Exception ex)
            {
                return Results.Problem(ex.Message);
            }
        }

        private static async Task<IResult> GetReportExecutions(
            Guid companyId,
            HttpContext httpContext,
            IMediator mediator,
            int page = 1,
            int pageSize = 20)
        {
            try
            {
                var claims = httpContext.ExtractTokenClaims();
                var query = new GetReportExecutionsQuery
                {
                    CompanyId = companyId,
                    UserId = Guid.Parse(claims.UserId!),
                    PageNumber = page,
                    PageSize = pageSize
                };

                var result = await mediator.Send(query);
                return Results.Ok(result);
            }
            catch (Exception ex)
            {
                return Results.Problem(ex.Message);
            }
        }

        private static async Task<IResult> ExportReportExecution(
            Guid companyId,
            Guid executionId,
            ExportFormat format,
            HttpContext httpContext,
            IMediator mediator)
        {
            try
            {
                var claims = httpContext.ExtractTokenClaims();
                var command = new CompanyService.Core.Feature.Commands.DynamicReports.ExportReportExecutionCommand
                {
                    ExecutionId = executionId,
                    Format = format,
                    UserId = Guid.Parse(claims.UserId!),
                    CompanyId = companyId
                };

                var result = await mediator.Send(command);
                return Results.File(result.Content, result.ContentType, result.FileName);
            }
            catch (ArgumentException ex)
            {
                return Results.NoContent();
            }
            catch (UnauthorizedAccessException ex)
            {
                return Results.Forbid();
            }
            catch (Exception ex)
            {
                return Results.Problem(ex.Message);
            }
        }

        private static async Task<IResult> GetAvailableEntities(
            Guid companyId,
            HttpContext httpContext,
            IMediator mediator)
        {
            try
            {
                var claims = httpContext.ExtractTokenClaims();
                var query = new GetAvailableEntitiesQuery
                {
                    CompanyId = companyId,
                    UserId = Guid.Parse(claims.UserId!)
                };

                var result = await mediator.Send(query);
                return Results.Ok(result);
            }
            catch (Exception ex)
            {
                return Results.Problem(ex.Message);
            }
        }

        private static async Task<IResult> GetEntityFields(
            Guid companyId,
            string entityName,
            HttpContext httpContext,
            IMediator mediator)
        {
            try
            {
                var claims = httpContext.ExtractTokenClaims();
                var query = new GetEntityFieldsQuery
                {
                    EntityName = entityName,
                    CompanyId = companyId,
                    UserId = Guid.Parse(claims.UserId!)
                };

                var result = await mediator.Send(query);
                return Results.Ok(result);
            }
            catch (ArgumentException ex)
            {
                return Results.BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return Results.Problem(ex.Message);
            }
        }
    }
}