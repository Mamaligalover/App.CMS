using System.Security.Claims;

namespace App.CMS.Application.Services;

public interface IAuthenticationService
{
    Task<AuthenticationResult> AuthenticateAsync(string email, string password);
    Task<bool> ValidateSessionAsync(ClaimsPrincipal principal);
    Task RecordLoginAttemptAsync(string email, bool success, string? ipAddress = null);
    Task<bool> IsAccountLockedAsync(string email);
    Task<int> GetFailedLoginAttemptsAsync(string email);
}

public class AuthenticationResult
{
    public bool Success { get; set; }
    public Guid? UserId { get; set; }
    public string? Username { get; set; }
    public string? Email { get; set; }
    public List<Claim> Claims { get; set; } = new();
    public string? ErrorMessage { get; set; }
    public bool IsLocked { get; set; }
    public int? RemainingAttempts { get; set; }
}