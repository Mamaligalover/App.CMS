using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using App.CMS.Data.Context;

namespace App.CMS.Application.Services;

public class AuthenticationService : IAuthenticationService
{
    private readonly AppDbContext _context;
    private readonly ILogger<AuthenticationService> _logger;

    public AuthenticationService(AppDbContext context, ILogger<AuthenticationService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<AuthenticationResult> AuthenticateAsync(string email, string password)
    {
        try
        {
            // Find user by email
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Email.ToLower() == email.ToLower());

            if (user == null)
            {
                _logger.LogWarning("Login attempt for non-existent email: {Email}", email);
                return new AuthenticationResult
                {
                    Success = false,
                    ErrorMessage = "Invalid email or password"
                };
            }

            // Check if user is active
            if (!user.IsActive)
            {
                _logger.LogWarning("Login attempt for inactive user: {Email}", email);
                return new AuthenticationResult
                {
                    Success = false,
                    ErrorMessage = "Your account has been deactivated. Please contact support."
                };
            }

            // Verify password
            if (!BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
            {
                _logger.LogWarning("Failed login attempt for user: {Email}", email);
                return new AuthenticationResult
                {
                    Success = false,
                    ErrorMessage = "Invalid email or password"
                };
            }

            // Update last login time
            user.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            // Create claims
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.GivenName, user.FirstName),
                new Claim(ClaimTypes.Surname, user.LastName),
                new Claim("FullName", $"{user.FirstName} {user.LastName}")
            };

            _logger.LogInformation("Successful login for user {Email} at {Time}", email, DateTime.UtcNow);

            return new AuthenticationResult
            {
                Success = true,
                UserId = user.Id,
                Username = user.Username,
                Email = user.Email,
                Claims = claims
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during authentication for email: {Email}", email);
            return new AuthenticationResult
            {
                Success = false,
                ErrorMessage = "An error occurred during authentication. Please try again."
            };
        }
    }

    public async Task<bool> ValidateSessionAsync(ClaimsPrincipal principal)
    {
        if (principal?.Identity?.IsAuthenticated != true)
            return false;

        var userIdClaim = principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (!Guid.TryParse(userIdClaim, out var userId))
            return false;

        var user = await _context.Users.FindAsync(userId);
        return user != null && user.IsActive;
    }

    public Task RecordLoginAttemptAsync(string email, bool success, string? ipAddress = null)
    {
        // Login attempts are not being tracked
        return Task.CompletedTask;
    }

    public Task<bool> IsAccountLockedAsync(string email)
    {
        // Account locking is not implemented
        return Task.FromResult(false);
    }

    public Task<int> GetFailedLoginAttemptsAsync(string email)
    {
        // Login attempts are not being tracked
        return Task.FromResult(0);
    }
}