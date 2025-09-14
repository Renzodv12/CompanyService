using CompanyService.Application.Commands.Finance;
using CompanyService.Application.Queries.Finance;
using CompanyService.Core.Attributes;
using CompanyService.Core.Services;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace CompanyService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FinanceController : ControllerBase
    {
        private readonly IMediator _mediator;

        public FinanceController(IMediator mediator)
        {
            _mediator = mediator;
        }

        // Accounts Receivable Endpoints
        [HttpPost("accounts-receivable")]
        [CacheInvalidate(CacheDataTypes.AccountsReceivable, invalidateCompanyData: true)]
        public async Task<IActionResult> CreateAccountsReceivable([FromBody] CreateAccountsReceivableCommand command)
        {
            var result = await _mediator.Send(command);
            return Ok(result);
        }

        [HttpGet("accounts-receivable/company/{companyId}")]
        [Cache(durationInMinutes: 20, cacheKeyPrefix: "accounts-receivable", varyByCompany: true)]
        public async Task<IActionResult> GetAccountsReceivableByCompany(Guid companyId, [FromQuery] bool onlyOverdue = false)
        {
            var query = new GetAccountsReceivableByCompanyQuery
            {
                CompanyId = companyId,
                OnlyOverdue = onlyOverdue
            };
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        [HttpPost("accounts-receivable/payment")]
        [CacheInvalidate(CacheDataTypes.AccountsReceivable, invalidateCompanyData: true)]
        public async Task<IActionResult> CreateAccountsReceivablePayment([FromBody] CreateAccountsReceivablePaymentCommand command)
        {
            var result = await _mediator.Send(command);
            return Ok(result);
        }

        // Bank Account Endpoints
        [HttpPost("bank-accounts")]
        [CacheInvalidate(CacheDataTypes.BankAccounts, invalidateCompanyData: true)]
        public async Task<IActionResult> CreateBankAccount([FromBody] CreateBankAccountCommand command)
        {
            var result = await _mediator.Send(command);
            return Ok(result);
        }

        [HttpGet("bank-accounts/company/{companyId}")]
        [Cache(durationInMinutes: 60, cacheKeyPrefix: "bank-accounts", varyByCompany: true)]
        public async Task<IActionResult> GetBankAccountsByCompany(Guid companyId)
        {
            var query = new GetBankAccountsByCompanyQuery { CompanyId = companyId };
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        // Bank Transaction Endpoints
        [HttpPost("bank-transactions")]
        [CacheInvalidate(CacheDataTypes.BankAccounts, invalidateCompanyData: true, customPatterns: new[] { "cash-flows:*" })]
        public async Task<IActionResult> CreateBankTransaction([FromBody] CreateBankTransactionCommand command)
        {
            var result = await _mediator.Send(command);
            return Ok(result);
        }

        // Cash Flow Endpoints
        [HttpPost("cash-flows")]
        [CacheInvalidate(CacheDataTypes.CashFlows, invalidateCompanyData: true)]
        public async Task<IActionResult> CreateCashFlow([FromBody] CreateCashFlowCommand command)
        {
            var result = await _mediator.Send(command);
            return Ok(result);
        }

        [HttpGet("cash-flows/company/{companyId}")]
        [Cache(durationInMinutes: 15, cacheKeyPrefix: "cash-flows", varyByCompany: true)]
        public async Task<IActionResult> GetCashFlowsByCompany(Guid companyId, [FromQuery] DateTime? startDate = null, [FromQuery] DateTime? endDate = null)
        {
            var query = new GetCashFlowsByCompanyQuery
            {
                CompanyId = companyId,
                StartDate = startDate,
                EndDate = endDate
            };
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        // Budget Endpoints
        [HttpPost("budgets")]
        public async Task<IActionResult> CreateBudget([FromBody] CreateBudgetCommand command)
        {
            var result = await _mediator.Send(command);
            return Ok(result);
        }
    }
}