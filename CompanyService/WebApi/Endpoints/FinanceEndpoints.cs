using CompanyService.Core.Feature.Commands.Finance;
using CompanyService.Application.Commands.Finance;
using CompanyService.Core.Feature.Querys.Finance;
using CompanyService.Application.Queries.Finance;
using CompanyService.Core.Models.Finance;
using CompanyService.Core.DTOs.Finance;
using CompanyService.Application.DTOs.Finance;
using CompanyService.WebApi.Extensions;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace CompanyService.WebApi.Endpoints
{
    public static class FinanceEndpoints
    {
        public static WebApplication MapFinanceEndpoints(this WebApplication app)
        {
            // Accounts Receivable Endpoints
            app.MapPost("/companies/{companyId:guid}/finance/accounts-receivable", CreateAccountReceivable)
                .WithName("CreateAccountReceivable")
                .WithTags("Finance")
                .RequireAuthorization()
                .Accepts<CreateAccountReceivableRequest>("application/json")
                .Produces(StatusCodes.Status201Created)
                .WithOpenApi();

            app.MapGet("/companies/{companyId:guid}/finance/accounts-receivable", GetAccountsReceivable)
                .WithName("GetAccountsReceivable")
                .WithTags("Finance")
                .RequireAuthorization()
                .Produces<List<AccountsReceivableResponseDto>>(StatusCodes.Status200OK)
                .WithOpenApi();

            app.MapGet("/companies/{companyId:guid}/finance/accounts-receivable/{id:guid}", GetAccountReceivable)
                .WithName("GetAccountReceivable")
                .WithTags("Finance")
                .RequireAuthorization()
                .Produces<AccountsReceivableResponseDto>(StatusCodes.Status200OK)
                .WithOpenApi();

            app.MapPost("/companies/{companyId:guid}/finance/accounts-receivable/{id:guid}/payments", CreateAccountReceivablePayment)
                .WithName("CreateAccountReceivablePayment")
                .WithTags("Finance")
                .RequireAuthorization()
                .Accepts<CreatePaymentRequest>("application/json")
                .Produces(StatusCodes.Status201Created)
                .WithOpenApi();

            // Accounts Payable Endpoints
            app.MapPost("/companies/{companyId:guid}/finance/accounts-payable", CreateAccountPayable)
                .WithName("CreateAccountPayable")
                .WithTags("Finance")
                .RequireAuthorization()
                .Accepts<CreateAccountPayableRequest>("application/json")
                .Produces(StatusCodes.Status201Created)
                .WithOpenApi();

            app.MapGet("/companies/{companyId:guid}/finance/accounts-payable", GetAccountsPayable)
                .WithName("GetAccountsPayable")
                .WithTags("Finance")
                .RequireAuthorization()
                .Produces<List<AccountsPayableResponseDto>>(StatusCodes.Status200OK)
                .WithOpenApi();

            app.MapGet("/companies/{companyId:guid}/finance/accounts-payable/{id:guid}", GetAccountPayable)
                .WithName("GetAccountPayable")
                .WithTags("Finance")
                .RequireAuthorization()
                .Produces<AccountsPayableResponseDto>(StatusCodes.Status200OK)
                .WithOpenApi();

            app.MapPost("/companies/{companyId:guid}/finance/accounts-payable/{id:guid}/payments", CreateAccountPayablePayment)
                .WithName("CreateAccountPayablePayment")
                .WithTags("Finance")
                .RequireAuthorization()
                .Accepts<CreatePaymentRequest>("application/json")
                .Produces(StatusCodes.Status201Created)
                .WithOpenApi();

            app.MapPut("/companies/{companyId:guid}/finance/accounts-payable/{id:guid}", UpdateAccountPayable)
                .WithName("UpdateAccountPayable")
                .WithTags("Finance")
                .RequireAuthorization()
                .Accepts<UpdateAccountPayableRequest>("application/json")
                .Produces(StatusCodes.Status200OK)
                .WithOpenApi();

            // Budget Endpoints
            app.MapPost("/companies/{companyId:guid}/finance/budgets", CreateBudget)
                .WithName("CreateBudget")
                .WithTags("Finance")
                .RequireAuthorization()
                .Accepts<CreateBudgetRequest>("application/json")
                .Produces(StatusCodes.Status201Created)
                .WithOpenApi();

            app.MapGet("/companies/{companyId:guid}/finance/budgets", GetBudgets)
                .WithName("GetBudgets")
                .WithTags("Finance")
                .RequireAuthorization()
                .Produces<List<BudgetResponseDto>>(StatusCodes.Status200OK)
                .WithOpenApi();

            app.MapGet("/companies/{companyId:guid}/finance/budgets/{id:guid}", GetBudget)
                .WithName("GetBudget")
                .WithTags("Finance")
                .RequireAuthorization()
                .Produces<BudgetResponseDto>(StatusCodes.Status200OK)
                .WithOpenApi();

            app.MapPut("/companies/{companyId:guid}/finance/budgets/{id:guid}", UpdateBudget)
                .WithName("UpdateBudget")
                .WithTags("Finance")
                .RequireAuthorization()
                .Accepts<UpdateBudgetRequest>("application/json")
                .Produces(StatusCodes.Status200OK)
                .WithOpenApi();

            app.MapDelete("/companies/{companyId:guid}/finance/budgets/{id:guid}", DeleteBudget)
                .WithName("DeleteBudget")
                .WithTags("Finance")
                .RequireAuthorization()
                .Produces(StatusCodes.Status200OK)
                .WithOpenApi();

            // Budget Summary Endpoint
            app.MapGet("/companies/{companyId:guid}/finance/budgets/summary", GetBudgetSummary)
                .WithName("GetBudgetSummary")
                .WithTags("Finance")
                .RequireAuthorization()
                .Produces<BudgetSummaryDto>(StatusCodes.Status200OK)
                .WithOpenApi();

            // Duplicate Budget Endpoint
            app.MapPost("/companies/{companyId:guid}/finance/budgets/{id:guid}/duplicate", DuplicateBudget)
                .WithName("DuplicateBudget")
                .WithTags("Finance")
                .RequireAuthorization()
                .Accepts<DuplicateBudgetRequest>("application/json")
                .Produces<BudgetResponseDto>(StatusCodes.Status201Created)
                .WithOpenApi();

            // Compare Budgets Endpoint
            app.MapPost("/companies/{companyId:guid}/finance/budgets/compare", CompareBudgets)
                .WithName("CompareBudgets")
                .WithTags("Finance")
                .RequireAuthorization()
                .Accepts<CompareBudgetsRequest>("application/json")
                .Produces<BudgetComparisonDto>(StatusCodes.Status200OK)
                .WithOpenApi();

            // Sales Financial Summary Endpoint
            app.MapGet("/companies/{companyId:guid}/finance/sales-summary", GetSalesFinancialSummary)
                .WithName("GetSalesFinancialSummary")
                .WithTags("Finance")
                .RequireAuthorization()
                .Produces<SalesFinancialSummaryDto>(StatusCodes.Status200OK)
                .WithOpenApi();

            // Financial Reports Endpoints
            app.MapGet("/companies/{companyId:guid}/finance/reports/profit-loss", GetProfitLossReport)
                .WithName("GetProfitLossReport")
                .WithTags("Finance")
                .RequireAuthorization()
                .Produces<ProfitLossReportDto>(StatusCodes.Status200OK)
                .WithOpenApi();

            app.MapGet("/companies/{companyId:guid}/finance/reports/balance-sheet", GetBalanceSheetReport)
                .WithName("GetBalanceSheetReport")
                .WithTags("Finance")
                .RequireAuthorization()
                .Produces<BalanceSheetReportDto>(StatusCodes.Status200OK)
                .WithOpenApi();

            app.MapGet("/companies/{companyId:guid}/finance/reports/cash-flow", GetCashFlowReport)
                .WithName("GetCashFlowReport")
                .WithTags("Finance")
                .RequireAuthorization()
                .Produces<CashFlowReportDto>(StatusCodes.Status200OK)
                .WithOpenApi();

            return app;
        }

        private static async Task<IResult> CreateAccountReceivable(
            Guid companyId,
            CreateAccountReceivableRequest request,
            HttpContext httpContext,
            ISender mediator)
        {
            var claims = httpContext.ExtractTokenClaims();
            if (claims.UserId is null)
                return Results.Unauthorized();

            var command = new CreateAccountReceivableCommand
            {
                CustomerId = request.CustomerId,
                InvoiceNumber = request.InvoiceNumber,
                Amount = request.TotalAmount,
                DueDate = request.DueDate,
                Description = request.Description ?? string.Empty,
                CompanyId = companyId,
                UserId = Guid.Parse(claims.UserId!)
            };

            try
            {
                var accountReceivableId = await mediator.Send(command);
                return Results.Created($"/companies/{companyId}/finance/accounts-receivable/{accountReceivableId}", new { Id = accountReceivableId });
            }
            catch (Exception ex)
            {
                return Results.BadRequest(new { error = ex.Message });
            }
        }

        private static async Task<IResult> GetAccountsReceivable(
            Guid companyId,
            HttpContext httpContext,
            ISender mediator)
        {
            var claims = httpContext.ExtractTokenClaims();
            if (claims.UserId is null)
                return Results.Unauthorized();

            var query = new GetAccountsReceivableQuery
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

        private static async Task<IResult> GetAccountReceivable(
            Guid companyId,
            Guid id,
            HttpContext httpContext,
            ISender mediator)
        {
            var claims = httpContext.ExtractTokenClaims();
            if (claims.UserId is null)
                return Results.Unauthorized();

            var query = new GetAccountReceivableQuery
            {
                Id = id,
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

        private static async Task<IResult> CreateAccountReceivablePayment(
            Guid companyId,
            Guid id,
            CreatePaymentRequest request,
            HttpContext httpContext,
            ISender mediator)
        {
            var claims = httpContext.ExtractTokenClaims();
            if (claims.UserId is null)
                return Results.Unauthorized();

            var command = new CreateAccountReceivablePaymentCommand
            {
                AccountReceivableId = id,
                Amount = request.Amount,
                PaymentDate = request.PaymentDate,
                PaymentMethod = request.PaymentMethod.ToString(),
                Reference = request.ReferenceNumber ?? string.Empty,
                CompanyId = companyId,
                UserId = Guid.Parse(claims.UserId!)
            };

            try
            {
                var paymentId = await mediator.Send(command);
                return Results.Created($"/companies/{companyId}/finance/accounts-receivable/{id}/payments/{paymentId}", new { Id = paymentId });
            }
            catch (Exception ex)
            {
                return Results.BadRequest(new { error = ex.Message });
            }
        }

        private static async Task<IResult> CreateAccountPayable(
            Guid companyId,
            CreateAccountPayableRequest request,
            HttpContext httpContext,
            ISender mediator)
        {
            var claims = httpContext.ExtractTokenClaims();
            if (claims.UserId is null)
                return Results.Unauthorized();

            var command = new CreateAccountPayableCommand
            {
                SupplierId = request.SupplierId,
                InvoiceNumber = request.InvoiceNumber,
                Amount = request.TotalAmount,
                DueDate = request.DueDate,
                Description = request.Description ?? string.Empty,
                CompanyId = companyId,
                UserId = Guid.Parse(claims.UserId!)
            };

            try
            {
                var accountPayableId = await mediator.Send(command);
                return Results.Created($"/companies/{companyId}/finance/accounts-payable/{accountPayableId}", new { Id = accountPayableId });
            }
            catch (Exception ex)
            {
                return Results.BadRequest(new { error = ex.Message });
            }
        }

        private static async Task<IResult> GetAccountsPayable(
            Guid companyId,
            HttpContext httpContext,
            ISender mediator)
        {
            var claims = httpContext.ExtractTokenClaims();
            if (claims.UserId is null)
                return Results.Unauthorized();

            var query = new GetAccountsPayableQuery
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

        private static async Task<IResult> GetAccountPayable(
            Guid companyId,
            Guid id,
            HttpContext httpContext,
            ISender mediator)
        {
            var claims = httpContext.ExtractTokenClaims();
            if (claims.UserId is null)
                return Results.Unauthorized();

            var query = new GetAccountPayableQuery
            {
                Id = id,
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

        private static async Task<IResult> CreateAccountPayablePayment(
            Guid companyId,
            Guid id,
            CreatePaymentRequest request,
            HttpContext httpContext,
            ISender mediator)
        {
            var claims = httpContext.ExtractTokenClaims();
            if (claims.UserId is null)
                return Results.Unauthorized();

            var command = new CreateAccountPayablePaymentCommand
            {
                AccountPayableId = id,
                Amount = request.Amount,
                PaymentDate = request.PaymentDate,
                PaymentMethod = request.PaymentMethod.ToString(),
                Reference = request.ReferenceNumber ?? string.Empty,
                CompanyId = companyId,
                UserId = Guid.Parse(claims.UserId!)
            };

            try
            {
                var paymentId = await mediator.Send(command);
                return Results.Created($"/companies/{companyId}/finance/accounts-payable/{id}/payments/{paymentId}", new { Id = paymentId });
            }
            catch (Exception ex)
            {
                return Results.BadRequest(new { error = ex.Message });
            }
        }

        private static async Task<IResult> CreateBudget(
            Guid companyId,
            CreateBudgetRequest request,
            HttpContext httpContext,
            ISender mediator)
        {
            var claims = httpContext.ExtractTokenClaims();
            if (claims.UserId is null)
                return Results.Unauthorized();

            var command = new CompanyService.Application.Commands.Finance.CreateBudgetCommand
            {
                Name = request.Name,
                Description = request.Description,
                Year = request.Year,
                Month = request.StartDate.Month,
                BudgetedAmount = request.TotalAmount,
                Category = "General",
                Notes = "",
                CompanyId = companyId,
                UserId = Guid.Parse(claims.UserId!),
                BudgetLines = request.Lines.Select(line => new CompanyService.Application.Commands.Finance.CreateBudgetLineCommand
                {
                    Description = $"Account {line.AccountId}",
                    BudgetedAmount = line.Amount,
                    Category = "General",
                    Notes = line.Notes ?? ""
                }).ToList()
            };

            try
            {
                var budget = await mediator.Send(command);
                return Results.Created($"/companies/{companyId}/finance/budgets/{budget.Id}", budget);
            }
            catch (Exception ex)
            {
                return Results.BadRequest(new { error = ex.Message });
            }
        }

        private static async Task<IResult> GetBudgets(
            Guid companyId,
            HttpContext httpContext,
            ISender mediator)
        {
            var claims = httpContext.ExtractTokenClaims();
            if (claims.UserId is null)
                return Results.Unauthorized();

            var query = new GetBudgetsQuery
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

        private static async Task<IResult> GetBudget(
            Guid companyId,
            Guid id,
            HttpContext httpContext,
            ISender mediator)
        {
            var claims = httpContext.ExtractTokenClaims();
            if (claims.UserId is null)
                return Results.Unauthorized();

            var query = new GetBudgetQuery
            {
                Id = id,
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

        private static async Task<IResult> UpdateBudget(
            Guid companyId,
            Guid id,
            UpdateBudgetRequest request,
            HttpContext httpContext,
            ISender mediator)
        {
            var claims = httpContext.ExtractTokenClaims();
            if (claims.UserId is null)
                return Results.Unauthorized();

            var command = new UpdateBudgetCommand
            {
                Id = id,
                Name = request.Name,
                Description = request.Description,
                Year = request.Year,
                Month = request.Month,
                BudgetedAmount = request.BudgetedAmount,
                AccountId = request.AccountId,
                Category = request.Category,
                Notes = request.Notes,
                BudgetLines = request.BudgetLines,
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

        private static async Task<IResult> GetProfitLossReport(
            Guid companyId,
            DateTime startDate,
            DateTime endDate,
            HttpContext httpContext,
            ISender mediator)
        {
            var claims = httpContext.ExtractTokenClaims();
            if (claims.UserId is null)
                return Results.Unauthorized();

            var query = new GetProfitLossReportQuery
            {
                CompanyId = companyId,
                StartDate = startDate,
                EndDate = endDate,
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

        private static async Task<IResult> GetBalanceSheetReport(
            Guid companyId,
            DateTime asOfDate,
            HttpContext httpContext,
            ISender mediator)
        {
            var claims = httpContext.ExtractTokenClaims();
            if (claims.UserId is null)
                return Results.Unauthorized();

            var query = new GetBalanceSheetReportQuery
            {
                CompanyId = companyId,
                AsOfDate = asOfDate,
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

        private static async Task<IResult> GetCashFlowReport(
            Guid companyId,
            DateTime startDate,
            DateTime endDate,
            HttpContext httpContext,
            ISender mediator)
        {
            var claims = httpContext.ExtractTokenClaims();
            if (claims.UserId is null)
                return Results.Unauthorized();

            var query = new GetCashFlowReportQuery
            {
                CompanyId = companyId,
                StartDate = startDate,
                EndDate = endDate,
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

        private static async Task<IResult> DeleteBudget(
            Guid companyId,
            Guid id,
            HttpContext httpContext,
            ISender mediator)
        {
            var claims = httpContext.ExtractTokenClaims();
            if (claims.UserId is null)
                return Results.Unauthorized();

            var command = new DeleteBudgetCommand
            {
                Id = id,
                CompanyId = companyId,
                UserId = Guid.Parse(claims.UserId!)
            };

            try
            {
                var result = await mediator.Send(command);
                return Results.Ok(new { success = result });
            }
            catch (Exception ex)
            {
                return Results.BadRequest(new { error = ex.Message });
            }
        }

        private static async Task<IResult> GetSalesFinancialSummary(
            Guid companyId,
            HttpContext httpContext,
            ISender mediator,
            DateTime? fromDate = null,
            DateTime? toDate = null)
        {
            var claims = httpContext.ExtractTokenClaims();
            if (claims.UserId is null)
                return Results.Unauthorized();

            var query = new GetSalesFinancialSummaryQuery
            {
                CompanyId = companyId,
                FromDate = fromDate,
                ToDate = toDate
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

        private static async Task<IResult> UpdateAccountPayable(
            Guid companyId,
            Guid id,
            UpdateAccountPayableRequest request,
            HttpContext httpContext,
            ISender mediator)
        {
            var claims = httpContext.ExtractTokenClaims();
            if (claims.UserId is null)
                return Results.Unauthorized();

            var command = new UpdateAccountPayableCommand
            {
                Id = id,
                SupplierId = request.SupplierId,
                InvoiceNumber = request.InvoiceNumber,
                Amount = request.TotalAmount,
                DueDate = request.DueDate,
                Description = request.Description ?? string.Empty,
                Notes = request.Notes ?? string.Empty,
                CompanyId = companyId,
                UserId = Guid.Parse(claims.UserId!)
            };

            try
            {
                var result = await mediator.Send(command);
                return Results.Ok(new { success = result });
            }
            catch (Exception ex)
            {
                return Results.BadRequest(new { error = ex.Message });
            }
        }

        private static async Task<IResult> GetBudgetSummary(
            Guid companyId,
            HttpContext httpContext,
            ISender mediator,
            int? year = null,
            int? month = null,
            DateTime? startDate = null,
            DateTime? endDate = null)
        {
            var claims = httpContext.ExtractTokenClaims();
            if (claims.UserId is null)
                return Results.Unauthorized();

            var query = new GetBudgetSummaryQuery
            {
                CompanyId = companyId,
                UserId = Guid.Parse(claims.UserId!),
                Year = year,
                Month = month,
                StartDate = startDate,
                EndDate = endDate
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

        private static async Task<IResult> DuplicateBudget(
            Guid companyId,
            Guid id,
            DuplicateBudgetRequest request,
            HttpContext httpContext,
            ISender mediator)
        {
            var claims = httpContext.ExtractTokenClaims();
            if (claims.UserId is null)
                return Results.Unauthorized();

            var command = new DuplicateBudgetCommand
            {
                Id = id,
                NewName = request.NewName,
                NewYear = request.NewYear,
                NewMonth = request.NewMonth,
                NewStartDate = request.NewStartDate,
                NewEndDate = request.NewEndDate,
                CompanyId = companyId,
                UserId = Guid.Parse(claims.UserId!)
            };

            try
            {
                var result = await mediator.Send(command);
                return Results.Created($"/companies/{companyId}/finance/budgets/{result.Id}", result);
            }
            catch (Exception ex)
            {
                return Results.BadRequest(new { error = ex.Message });
            }
        }

        private static async Task<IResult> CompareBudgets(
            Guid companyId,
            CompareBudgetsRequest request,
            HttpContext httpContext,
            ISender mediator)
        {
            var claims = httpContext.ExtractTokenClaims();
            if (claims.UserId is null)
                return Results.Unauthorized();

            var command = new CompareBudgetsCommand
            {
                BudgetIds = request.BudgetIds,
                CompanyId = companyId,
                UserId = Guid.Parse(claims.UserId!)
            };

            try
            {
                var result = await mediator.Send(command);
                return Results.Ok(result);
            }
            catch (Exception ex)
            {
                return Results.BadRequest(new { error = ex.Message });
            }
        }
    }
}