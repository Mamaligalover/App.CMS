using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MediatR;
using App.CMS.Application.Commands.Auth.Login;
using System.Security.Claims;
using System.ComponentModel.DataAnnotations;

namespace App.CMS.Pages.Account;

public class LoginModel : PageModel
{
    private readonly IMediator _mediator;
    private readonly ILogger<LoginModel> _logger;

    public LoginModel(IMediator mediator, ILogger<LoginModel> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    [BindProperty]
    public InputModel Input { get; set; } = new();

    public class InputModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;

        [Display(Name = "Remember me?")]
        public bool RememberMe { get; set; }

        public string? ReturnUrl { get; set; }
    }

    public async Task OnGetAsync(string? returnUrl = null)
    {
        Input.ReturnUrl = returnUrl ?? Url.Content("~/");
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
    }

    public async Task<IActionResult> OnPostAsync()
    {
        Input.ReturnUrl ??= Url.Content("~/");

        if (ModelState.IsValid)
        {
            try
            {
                // Get client IP address
                var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString();

                var command = new LoginCommand
                {
                    Email = Input.Email,
                    Password = Input.Password
                };

                var result = await _mediator.Send(command);

                if (result.Success)
                {
                    var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.NameIdentifier, result.UserId.ToString()),
                        new Claim(ClaimTypes.Name, result.Username),
                        new Claim(ClaimTypes.Email, Input.Email)
                    };

                    var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                    var authProperties = new AuthenticationProperties
                    {
                        IsPersistent = Input.RememberMe,
                        RedirectUri = Input.ReturnUrl,
                        ExpiresUtc = Input.RememberMe
                            ? DateTimeOffset.UtcNow.AddDays(7)
                            : DateTimeOffset.UtcNow.AddHours(2)
                    };

                    await HttpContext.SignInAsync(
                        CookieAuthenticationDefaults.AuthenticationScheme,
                        new ClaimsPrincipal(claimsIdentity),
                        authProperties);

                    _logger.LogInformation("User {Email} logged in successfully from IP {IpAddress} at {Time}",
                        Input.Email, ipAddress, DateTime.UtcNow);

                    return LocalRedirect(Input.ReturnUrl);
                }
                else
                {
                    _logger.LogWarning("Failed login attempt for {Email} from IP {IpAddress}. Error: {Error}",
                        Input.Email, ipAddress, result.ErrorMessage);

                    ModelState.AddModelError(string.Empty, result.ErrorMessage ?? "Invalid login attempt.");
                    return Page();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during login for user {Email}", Input.Email);
                ModelState.AddModelError(string.Empty, "An error occurred during login. Please try again.");
                return Page();
            }
        }

        return Page();
    }
}