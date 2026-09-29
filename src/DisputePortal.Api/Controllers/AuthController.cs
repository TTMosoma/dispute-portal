namespace DisputePortal.Api.Controllers;

using DisputePortal.Api.Auth;
using DisputePortal.Api.DataTransferObjects.Requests;
using DisputePortal.Api.DataTransferObjects.Responses;
using DisputePortal.Domain.Entities;
using DisputePortal.Infrastructure.Persistence;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[ApiController]
[Route("api/[controller]")]
public class AuthController(DisputePortalDbContext db, TokenService tokens) : ControllerBase
{
    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginRequest req)
    {
        var user = await db.User.SingleOrDefaultAsync(u => u.Email == req.Email);

        // Deliberately give a vague response to reduce information provided to malicious actors
        if (user is null)
            return Unauthorized("Invalid credentials.");
            
        var result = new PasswordHasher<User>()
            .VerifyHashedPassword(user, user.PasswordHash, req.Password);

        if (result == PasswordVerificationResult.Failed)
            return Unauthorized("Invalid credentials.");

        return Ok(new LoginResponse(tokens.CreateToken(user)));
    }
}