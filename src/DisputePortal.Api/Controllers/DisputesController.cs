using DisputePortal.Api.Auth;
using DisputePortal.Api.DataTransferObjects.Requests;
using DisputePortal.Api.DataTransferObjects.Responses;
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

        var existingUserTransaction = await db.Transactions.AnyAsync(t => t.Id == request.TransactionId && db.Accounts.Any(a => a.UserId == userId && a.Id == t.AccountId));

        if (!existingUserTransaction) return NotFound($"Transaction cannot be found!");

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

    [HttpPost("{id:guid}/withdraw")]
    [Authorize(Roles = "Customer")]
    public async Task<IActionResult> Withdraw(Guid id, TransitionRequest req)
    {
        var userId = User.GetUserId();
        var dispute = await db.Disputes.Include(d => d.StatusHistory)
            .SingleOrDefaultAsync(d => d.Id == id && d.CustomerId == userId);
        if (dispute is null) return NotFound();

        try
        {
            dispute.Withdraw(userId, req.Note);
            await db.SaveChangesAsync();
            return NoContent();
        }
        catch (InvalidDisputeTransitionException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPost("{id:guid}/reject")]
    [Authorize(Roles = "Agent")]
    public async Task<IActionResult> Reject(Guid id, TransitionRequest req)
    {
        var agentId = User.GetUserId();
        var dispute = await db.Disputes.Include(d => d.StatusHistory)
            .SingleOrDefaultAsync(d => d.Id == id); // Agents can act on any dispute.
        if (dispute is null) return NotFound();

        try
        {
            dispute.Reject(agentId, req.Note);
            await db.SaveChangesAsync();
            return NoContent();
        }
        catch (InvalidDisputeTransitionException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPost("{id:guid}/resolve")]
    [Authorize(Roles = "Agent")]
    public async Task<IActionResult> Resolve(Guid id, TransitionRequest req)
    {
        var agentId = User.GetUserId();
        var dispute = await db.Disputes.Include(d => d.StatusHistory)
            .SingleOrDefaultAsync(d => d.Id == id);
        if (dispute is null) return NotFound();

        try
        {
            dispute.Resolve(agentId, req.Note);
            await db.SaveChangesAsync();
            return NoContent();
        }
        catch (InvalidDisputeTransitionException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPost("{id:guid}/review")]
    [Authorize(Roles = "Agent")]
    public async Task<IActionResult> MoveToUnderReview(Guid id, TransitionRequest req)
    {
        var agentId = User.GetUserId();
        var dispute = await db.Disputes.Include(d => d.StatusHistory)
            .SingleOrDefaultAsync(d => d.Id == id);
        if (dispute is null) return NotFound();

        try
        {
            dispute.MoveToUnderReview(agentId, req.Note);
            foreach (var e in db.ChangeTracker.Entries())
                Console.WriteLine($"{e.Entity.GetType().Name} => {e.State} | Id={((dynamic)e.Entity).Id}");
            await db.SaveChangesAsync();
            return NoContent();
        }
        catch (InvalidDisputeTransitionException ex)
        {
            return BadRequest(ex.Message);
        }
    }
}