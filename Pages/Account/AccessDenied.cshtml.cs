using Microsoft.AspNetCore.Mvc.RazorPages;

namespace App.CMS.Pages.Account;

public class AccessDeniedModel : PageModel
{
    private readonly ILogger<AccessDeniedModel> _logger;

    public AccessDeniedModel(ILogger<AccessDeniedModel> logger)
    {
        _logger = logger;
    }

    public void OnGet()
    {
        _logger.LogWarning("Access denied for user {User} at {Time}", User.Identity?.Name ?? "Anonymous", DateTime.UtcNow);
    }
}