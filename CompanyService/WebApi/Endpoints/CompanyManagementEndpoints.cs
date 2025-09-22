using CompanyService.Core.Feature.Commands.CompanyManagement;
using CompanyService.Core.Feature.Querys.CompanyManagement;
using CompanyService.Core.Models.CompanyManagement;
using CompanyService.Core.DTOs;
using CompanyService.Core.DTOs.Branch;
using CompanyService.Core.DTOs.Department;
using CompanyService.WebApi.Extensions;
using MediatR;

namespace CompanyService.WebApi.Endpoints
{
    public static class CompanyManagementEndpoints
    {
        public static IEndpointRouteBuilder MapCompanyManagementEndpoints(this IEndpointRouteBuilder app)
        {
            // Branch Management Endpoints
            app.MapPost("/companies/{companyId:guid}/management/branches", CreateBranch)
                .WithName("CreateBranch")
                .WithTags("CompanyManagement")
                .RequireAuthorization()
                .Accepts<CompanyService.Core.Models.CompanyManagement.CreateBranchRequest>("application/json")
                .Produces(StatusCodes.Status201Created)
                .Produces(StatusCodes.Status400BadRequest)
                .WithOpenApi();

            app.MapGet("/companies/{companyId:guid}/management/branches", GetBranches)
                .WithName("GetBranches")
                .WithTags("CompanyManagement")
                .RequireAuthorization()
                .Produces<List<BranchDto>>(StatusCodes.Status200OK)
                .WithOpenApi();

            app.MapGet("/companies/{companyId:guid}/management/branches/{branchId:guid}", GetBranch)
                .WithName("GetBranch")
                .WithTags("CompanyManagement")
                .RequireAuthorization()
                .Produces<BranchDetailDto>(StatusCodes.Status200OK)
                .WithOpenApi();

            app.MapPut("/companies/{companyId:guid}/management/branches/{branchId:guid}", UpdateBranch)
                .WithName("UpdateBranch")
                .WithTags("CompanyManagement")
                .RequireAuthorization()
                .Accepts<CompanyService.Core.Models.CompanyManagement.UpdateBranchRequest>("application/json")
                .Produces(StatusCodes.Status200OK)
                .WithOpenApi();

            app.MapDelete("/companies/{companyId:guid}/management/branches/{branchId:guid}", DeleteBranch)
                .WithName("DeleteBranch")
                .WithTags("CompanyManagement")
                .RequireAuthorization()
                .Produces(StatusCodes.Status200OK)
                .WithOpenApi();

            // Department Management Endpoints
            app.MapPost("/companies/{companyId:guid}/management/departments", CreateDepartment)
                .WithName("CreateDepartment")
                .WithTags("CompanyManagement")
                .RequireAuthorization()
                .Accepts<CreateDepartmentRequest>("application/json")
                .Produces(StatusCodes.Status201Created)
                .WithOpenApi();

            app.MapGet("/companies/{companyId:guid}/management/departments", GetDepartments)
                .WithName("GetDepartments")
                .WithTags("CompanyManagement")
                .RequireAuthorization()
                .Produces<List<DepartmentDto>>(StatusCodes.Status200OK)
                .WithOpenApi();

            app.MapPut("/companies/{companyId:guid}/management/departments/{departmentId:guid}", UpdateDepartment)
                .WithName("UpdateDepartment")
                .WithTags("CompanyManagement")
                .RequireAuthorization()
                .Accepts<UpdateDepartmentRequest>("application/json")
                .Produces(StatusCodes.Status200OK)
                .WithOpenApi();

            app.MapDelete("/companies/{companyId:guid}/management/departments/{departmentId:guid}", DeleteDepartment)
                .WithName("DeleteDepartment")
                .WithTags("CompanyManagement")
                .RequireAuthorization()
                .Produces(StatusCodes.Status200OK)
                .WithOpenApi();

            // Position Management Endpoints
            app.MapPost("/companies/{companyId:guid}/management/positions", CreatePosition)
                .WithName("CreatePosition")
                .WithTags("CompanyManagement")
                .RequireAuthorization()
                .Accepts<CreatePositionRequest>("application/json")
                .Produces(StatusCodes.Status201Created)
                .WithOpenApi();

            app.MapGet("/companies/{companyId:guid}/management/positions", GetPositions)
                .WithName("GetPositions")
                .WithTags("CompanyManagement")
                .RequireAuthorization()
                .Produces<List<PositionDto>>(StatusCodes.Status200OK)
                .WithOpenApi();

            app.MapPut("/companies/{companyId:guid}/management/positions/{positionId:guid}", UpdatePosition)
                .WithName("UpdatePosition")
                .WithTags("CompanyManagement")
                .RequireAuthorization()
                .Accepts<UpdatePositionRequest>("application/json")
                .Produces(StatusCodes.Status200OK)
                .WithOpenApi();

            app.MapDelete("/companies/{companyId:guid}/management/positions/{positionId:guid}", DeletePosition)
                .WithName("DeletePosition")
                .WithTags("CompanyManagement")
                .RequireAuthorization()
                .Produces(StatusCodes.Status200OK)
                .WithOpenApi();

            // Employee Management Endpoints
            app.MapPost("/companies/{companyId:guid}/management/employees", CreateEmployee)
                .WithName("CreateEmployee")
                .WithTags("CompanyManagement")
                .RequireAuthorization()
                .Accepts<CreateEmployeeRequest>("application/json")
                .Produces(StatusCodes.Status201Created)
                .WithOpenApi();

            app.MapGet("/companies/{companyId:guid}/management/employees", GetEmployees)
                .WithName("GetEmployees")
                .WithTags("CompanyManagement")
                .RequireAuthorization()
                .Produces<List<EmployeeDto>>(StatusCodes.Status200OK)
                .WithOpenApi();

            app.MapGet("/companies/{companyId:guid}/management/employees/{employeeId:guid}", GetEmployee)
                .WithName("GetEmployee")
                .WithTags("CompanyManagement")
                .RequireAuthorization()
                .Produces<EmployeeDetailDto>(StatusCodes.Status200OK)
                .WithOpenApi();

            app.MapPut("/companies/{companyId:guid}/management/employees/{employeeId:guid}", UpdateEmployee)
                .WithName("UpdateEmployee")
                .WithTags("CompanyManagement")
                .RequireAuthorization()
                .Accepts<UpdateEmployeeRequest>("application/json")
                .Produces(StatusCodes.Status200OK)
                .WithOpenApi();

            app.MapDelete("/companies/{companyId:guid}/management/employees/{employeeId:guid}", DeleteEmployee)
                .WithName("DeleteEmployee")
                .WithTags("CompanyManagement")
                .RequireAuthorization()
                .Produces(StatusCodes.Status200OK)
                .WithOpenApi();

            return app;
        }

        private static async Task<IResult> CreateBranch(
            Guid companyId,
            CompanyService.Core.Models.CompanyManagement.CreateBranchRequest request,
            HttpContext httpContext,
            ISender mediator)
        {
            var claims = httpContext.ExtractTokenClaims();
            if (claims.UserId is null)
                return Results.Unauthorized();

            var command = new CreateBranchCommand
            {
                Name = request.Name,
                Address = request.Address,
                Phone = request.Phone,
                Email = request.Email,
                ManagerId = request.ManagerId,
                CompanyId = companyId,
                UserId = Guid.Parse(claims.UserId!)
            };

            try
            {
                var branchId = await mediator.Send(command);
                return Results.Created($"/companies/{companyId}/management/branches/{branchId}", new { Id = branchId });
            }
            catch (Exception ex)
            {
                return Results.BadRequest(new { error = ex.Message });
            }
        }

        private static async Task<IResult> GetBranches(
            Guid companyId,
            HttpContext httpContext,
            ISender mediator)
        {
            var claims = httpContext.ExtractTokenClaims();
            if (claims.UserId is null)
                return Results.Unauthorized();

            var query = new GetBranchesQuery
            {
                CompanyId = companyId,
                UserId = Guid.Parse(claims.UserId!)
            };

            try
            {
                var result = await mediator.Send(query);
                return Results.Ok(result);
            }
            catch (Exception ex)
            {
                return Results.BadRequest(new { error = ex.Message });
            }
        }

        private static async Task<IResult> GetBranch(
            Guid companyId,
            Guid branchId,
            HttpContext httpContext,
            ISender mediator)
        {
            var claims = httpContext.ExtractTokenClaims();
            if (claims.UserId is null)
                return Results.Unauthorized();

            var query = new GetBranchDetailQuery
            {
                BranchId = branchId,
                CompanyId = companyId,
                UserId = Guid.Parse(claims.UserId!)
            };

            try
            {
                var result = await mediator.Send(query);
                return Results.Ok(result);
            }
            catch (Exception ex)
            {
                return Results.BadRequest(new { error = ex.Message });
            }
        }

        private static async Task<IResult> UpdateBranch(
            Guid companyId,
            Guid branchId,
            CompanyService.Core.Models.CompanyManagement.UpdateBranchRequest request,
            HttpContext httpContext,
            ISender mediator)
        {
            var claims = httpContext.ExtractTokenClaims();
            if (claims.UserId is null)
                return Results.Unauthorized();

            var command = new UpdateBranchCommand
            {
                Id = branchId,
                Name = request.Name,
                Address = request.Address,
                Phone = request.Phone,
                Email = request.Email,
                ManagerId = request.ManagerId,
                CompanyId = companyId,
                UserId = Guid.Parse(claims.UserId!)
            };

            try
            {
                await mediator.Send(command);
                return Results.Ok();
            }
            catch (Exception ex)
            {
                return Results.BadRequest(new { error = ex.Message });
            }
        }

        private static async Task<IResult> DeleteBranch(
            Guid companyId,
            Guid branchId,
            HttpContext httpContext,
            ISender mediator)
        {
            var claims = httpContext.ExtractTokenClaims();
            if (claims.UserId is null)
                return Results.Unauthorized();

            var command = new DeleteBranchCommand
            {
                Id = branchId,
                CompanyId = companyId,
                UserId = Guid.Parse(claims.UserId!)
            };

            try
            {
                await mediator.Send(command);
                return Results.Ok();
            }
            catch (Exception ex)
            {
                return Results.BadRequest(new { error = ex.Message });
            }
        }

        private static async Task<IResult> CreateDepartment(
            Guid companyId,
            CreateDepartmentRequest request,
            HttpContext httpContext,
            ISender mediator)
        {
            var claims = httpContext.ExtractTokenClaims();
            if (claims.UserId is null)
                return Results.Unauthorized();

            var command = new CreateDepartmentCommand
            {
                Name = request.Name,
                Description = request.Description,
                BranchId = request.BranchId,
                ManagerId = request.ManagerId,
                CompanyId = companyId,
                UserId = Guid.Parse(claims.UserId!)
            };

            try
            {
                var departmentId = await mediator.Send(command);
                return Results.Created($"/companies/{companyId}/management/departments/{departmentId}", new { Id = departmentId });
            }
            catch (Exception ex)
            {
                return Results.BadRequest(new { error = ex.Message });
            }
        }

        private static async Task<IResult> GetDepartments(
            Guid companyId,
            HttpContext httpContext,
            ISender mediator)
        {
            var claims = httpContext.ExtractTokenClaims();
            if (claims.UserId is null)
                return Results.Unauthorized();

            var query = new GetDepartmentsQuery
            {
                CompanyId = companyId,
                UserId = Guid.Parse(claims.UserId!)
            };

            try
            {
                var result = await mediator.Send(query);
                return Results.Ok(result);
            }
            catch (Exception ex)
            {
                return Results.BadRequest(new { error = ex.Message });
            }
        }

        private static async Task<IResult> UpdateDepartment(
            Guid companyId,
            Guid departmentId,
            UpdateDepartmentRequest request,
            HttpContext httpContext,
            ISender mediator)
        {
            var claims = httpContext.ExtractTokenClaims();
            if (claims.UserId is null)
                return Results.Unauthorized();

            var command = new UpdateDepartmentCommand
            {
                Id = departmentId,
                Name = request.Name,
                Description = request.Description,
                ManagerId = request.ManagerId,
                CompanyId = companyId,
                UserId = Guid.Parse(claims.UserId!)
            };

            try
            {
                await mediator.Send(command);
                return Results.Ok();
            }
            catch (Exception ex)
            {
                return Results.BadRequest(new { error = ex.Message });
            }
        }

        private static async Task<IResult> DeleteDepartment(
            Guid companyId,
            Guid departmentId,
            HttpContext httpContext,
            ISender mediator)
        {
            var claims = httpContext.ExtractTokenClaims();
            if (claims.UserId is null)
                return Results.Unauthorized();

            var command = new DeleteDepartmentCommand
            {
                Id = departmentId,
                CompanyId = companyId,
                UserId = Guid.Parse(claims.UserId!)
            };

            try
            {
                await mediator.Send(command);
                return Results.Ok();
            }
            catch (Exception ex)
            {
                return Results.BadRequest(new { error = ex.Message });
            }
        }

        private static async Task<IResult> CreatePosition(
            Guid companyId,
            CreatePositionRequest request,
            HttpContext httpContext,
            ISender mediator)
        {
            var claims = httpContext.ExtractTokenClaims();
            if (claims.UserId is null)
                return Results.Unauthorized();

            var command = new CreatePositionCommand
            {
                Title = request.Name,
                Description = request.Description,
                CompanyId = companyId,
                UserId = Guid.Parse(claims.UserId!)
            };

            try
            {
                var positionId = await mediator.Send(command);
                return Results.Created($"/companies/{companyId}/management/positions/{positionId}", new { Id = positionId });
            }
            catch (Exception ex)
            {
                return Results.BadRequest(new { error = ex.Message });
            }
        }

        private static async Task<IResult> GetPositions(
            Guid companyId,
            HttpContext httpContext,
            ISender mediator)
        {
            var claims = httpContext.ExtractTokenClaims();
            if (claims.UserId is null)
                return Results.Unauthorized();

            var query = new GetPositionsQuery
            {
                CompanyId = companyId,
                UserId = Guid.Parse(claims.UserId!)
            };

            try
            {
                var result = await mediator.Send(query);
                return Results.Ok(result);
            }
            catch (Exception ex)
            {
                return Results.BadRequest(new { error = ex.Message });
            }
        }

        private static async Task<IResult> UpdatePosition(
            Guid companyId,
            Guid positionId,
            UpdatePositionRequest request,
            HttpContext httpContext,
            ISender mediator)
        {
            var claims = httpContext.ExtractTokenClaims();
            if (claims.UserId is null)
                return Results.Unauthorized();

            var command = new UpdatePositionCommand
            {
                Id = positionId,
                Title = request.Name,
                Description = request.Description,
                CompanyId = companyId,
                UserId = Guid.Parse(claims.UserId!)
            };

            try
            {
                await mediator.Send(command);
                return Results.Ok();
            }
            catch (Exception ex)
            {
                return Results.BadRequest(new { error = ex.Message });
            }
        }

        private static async Task<IResult> DeletePosition(
            Guid companyId,
            Guid positionId,
            HttpContext httpContext,
            ISender mediator)
        {
            var claims = httpContext.ExtractTokenClaims();
            if (claims.UserId is null)
                return Results.Unauthorized();

            var command = new DeletePositionCommand
            {
                Id = positionId,
                CompanyId = companyId,
                UserId = Guid.Parse(claims.UserId!)
            };

            try
            {
                await mediator.Send(command);
                return Results.Ok();
            }
            catch (Exception ex)
            {
                return Results.BadRequest(new { error = ex.Message });
            }
        }

        private static async Task<IResult> CreateEmployee(
            Guid companyId,
            CreateEmployeeRequest request,
            HttpContext httpContext,
            ISender mediator)
        {
            var claims = httpContext.ExtractTokenClaims();
            if (claims.UserId is null)
                return Results.Unauthorized();

            var command = new CreateEmployeeCommand
            {
                FirstName = request.FirstName,
                LastName = request.LastName,
                Email = request.Email,
                Phone = request.Phone,
                HireDate = request.HireDate,
                Salary = request.Salary,
                PositionId = request.PositionId,
                BranchId = request.BranchId,
                CompanyId = companyId,
                UserId = Guid.Parse(claims.UserId!)
            };

            try
            {
                var employeeId = await mediator.Send(command);
                return Results.Created($"/companies/{companyId}/management/employees/{employeeId}", new { Id = employeeId });
            }
            catch (Exception ex)
            {
                return Results.BadRequest(new { error = ex.Message });
            }
        }

        private static async Task<IResult> GetEmployees(
            Guid companyId,
            HttpContext httpContext,
            ISender mediator)
        {
            var claims = httpContext.ExtractTokenClaims();
            if (claims.UserId is null)
                return Results.Unauthorized();

            var query = new GetEmployeesQuery
            {
                CompanyId = companyId,
                UserId = Guid.Parse(claims.UserId!)
            };

            try
            {
                var result = await mediator.Send(query);
                return Results.Ok(result);
            }
            catch (Exception ex)
            {
                return Results.BadRequest(new { error = ex.Message });
            }
        }

        private static async Task<IResult> GetEmployee(
            Guid companyId,
            Guid employeeId,
            HttpContext httpContext,
            ISender mediator)
        {
            var claims = httpContext.ExtractTokenClaims();
            if (claims.UserId is null)
                return Results.Unauthorized();

            var query = new GetEmployeeDetailQuery
            {
                EmployeeId = employeeId,
                CompanyId = companyId,
                UserId = Guid.Parse(claims.UserId!)
            };

            try
            {
                var result = await mediator.Send(query);
                return Results.Ok(result);
            }
            catch (Exception ex)
            {
                return Results.BadRequest(new { error = ex.Message });
            }
        }

        private static async Task<IResult> UpdateEmployee(
            Guid companyId,
            Guid employeeId,
            UpdateEmployeeRequest request,
            HttpContext httpContext,
            ISender mediator)
        {
            var claims = httpContext.ExtractTokenClaims();
            if (claims.UserId is null)
                return Results.Unauthorized();

            var command = new UpdateEmployeeCommand
            {
                Id = employeeId,
                FirstName = request.FirstName ?? string.Empty,
                LastName = request.LastName ?? string.Empty,
                Email = request.Email ?? string.Empty,
                Phone = request.Phone ?? string.Empty,
                HireDate = request.HireDate ?? DateTime.Now,
                Salary = request.Salary ?? 0,
                PositionId = request.PositionId ?? Guid.Empty,
                BranchId = request.BranchId ?? Guid.Empty,
                CompanyId = companyId,
                UserId = Guid.Parse(claims.UserId!)
            };

            try
            {
                await mediator.Send(command);
                return Results.Ok();
            }
            catch (Exception ex)
            {
                return Results.BadRequest(new { error = ex.Message });
            }
        }

        private static async Task<IResult> DeleteEmployee(
            Guid companyId,
            Guid employeeId,
            HttpContext httpContext,
            ISender mediator)
        {
            var claims = httpContext.ExtractTokenClaims();
            if (claims.UserId is null)
                return Results.Unauthorized();

            var command = new DeleteEmployeeCommand
            {
                Id = employeeId,
                CompanyId = companyId,
                UserId = Guid.Parse(claims.UserId!)
            };

            try
            {
                await mediator.Send(command);
                return Results.Ok();
            }
            catch (Exception ex)
            {
                return Results.BadRequest(new { error = ex.Message });
            }
        }
    }
}