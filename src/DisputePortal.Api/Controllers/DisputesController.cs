using DisputePortal.Api.Auth;
using DisputePortal.Api.DataTransferObjects;
using DisputePortal.Api.DataTransferObjects.Requests;
using DisputePortal.Domain.Entities;
using DisputePortal.Infrastructure.Persistence;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DisputePortal.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class DisputesController(DisputePortalDbContext db) : ControllerBase
{
    [HttpPost]
    [Authorize(Roles = "Customer")]
    public async Task<IActionResult> Create(CreateDisputeRequest request)
    {
        var userId = User.GetUserId();

        var transactionBelongsToUser = await db.Transactions.AnyAsync(t =>
            t.Id == request.TransactionId &&
            db.Accounts.Any(a => a.Id == t.AccountId && a.UserId == userId));

        if (!transactionBelongsToUser) return NotFound("Transaction not found.");

        var dispute = new Dispute(request.TransactionId, userId, request.Category, request.Reason);
        db.Disputes.Add(dispute);

        try
        {
            await db.SaveChangesAsync();
        }
        catch (DbUpdateException)
        {
            return BadRequest("Unable to save this transaction.");
        }

        var response = new DisputeDto(dispute.Id, dispute.TransactionId, dispute.Category.ToString(), dispute.Status.ToString(), dispute.Reason, dispute.CreatedAt, dispute.UpdatedAt);

        return CreatedAtAction(nameof(GetById), new { id = dispute.Id }, response);
    }

    [HttpGet]
    [Authorize(Roles = "Customer")]
    public async Task<IActionResult> GetAllUserDisputes()
    {
        var userId = User.GetUserId();
        var disputes = await db.Disputes
            .Where(d => d.CustomerId == userId)
            .OrderByDescending(d => d.CreatedAt)
            .Select(d => new DisputeDto(d.Id, d.TransactionId, d.Category.ToString(), d.Status.ToString(), d.Reason, d.CreatedAt, d.UpdatedAt))
            .ToListAsync();

        return Ok(disputes);
    }

    [HttpGet("{id:guid}")]
    [Authorize(Roles = "Customer")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var userId = User.GetUserId();
        var dispute = await db.Disputes
            .Where(d => d.Id == id && d.CustomerId == userId)
            .Select(d => new DisputeDto(d.Id, d.TransactionId, d.Category.ToString(), d.Status.ToString(), d.Reason, d.CreatedAt, d.UpdatedAt))
            .SingleOrDefaultAsync();

        return dispute is null ? NotFound() : Ok(dispute);
    }

    [HttpGet("{id:guid}/history")]
    [Authorize(Roles = "Customer")]
    public async Task<IActionResult> GetDisputeHistory(Guid id)
    {
        var userId = User.GetUserId();

        var dispute = await db.Disputes.AnyAsync(d => d.Id == id && d.CustomerId == userId);
        if (!dispute) return NotFound();

        var history = await db.Set<DisputeStatusHistory>()
            .Where(h => h.DisputeId == id)
            .OrderBy(h => h.ChangedAt)
            .Select(h => new HistoryDto(h.FromStatus, h.ToStatus, h.ChangedBy, h.Note, h.ChangedAt))
            .ToListAsync();

        return Ok(history);
    }
}