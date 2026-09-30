using DisputePortal.Api.Auth;
using DisputePortal.Api.DataTransferObjects.Requests;
using DisputePortal.Infrastructure.Persistence;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DisputePortal.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Customer")]
public class TransactionsController(DisputePortalDbContext db) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetOwnTransactions()
    {
        var userId = User.GetUserId();

        var transactions = await db.Transactions
            .Where(t => db.Accounts.Any(a => a.Id == t.AccountId && a.UserId == userId)) 
            .OrderByDescending(t => t.TransactionDate)
            .Select(t => new TransactionDto()
            {
                Id = t.Id,
                Amount = t.Amount,
                Currency = t.Currency,
                TransactionType = t.TransactionType.ToString(),
                MerchantName = t.MerchantName,
                Description = t.Description,
                TransactionDate = t.TransactionDate
            })
            .ToListAsync();

        return Ok(transactions);
    }
}