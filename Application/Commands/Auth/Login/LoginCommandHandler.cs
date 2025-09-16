using MediatR;
using App.CMS.Application.Services;

namespace App.CMS.Application.Commands.Auth.Login;

public class LoginCommandHandler : IRequestHandler<LoginCommand, LoginResult>
{
    private readonly IAuthenticationService _authService;
    private readonly ILogger<LoginCommandHandler> _logger;

    public LoginCommandHandler(IAuthenticationService authService, ILogger<LoginCommandHandler> logger)
    {
        _authService = authService;
        _logger = logger;
    }

    public async Task<LoginResult> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var authResult = await _authService.AuthenticateAsync(request.Email, request.Password);

            return new LoginResult
            {
                Success = authResult.Success,
                UserId = authResult.UserId ?? Guid.Empty,
                Username = authResult.Username ?? string.Empty,
                Email = authResult.Email ?? string.Empty,
                ErrorMessage = authResult.ErrorMessage
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during login for email: {Email}", request.Email);
            return new LoginResult
            {
                Success = false,
                ErrorMessage = "An error occurred during login. Please try again."
            };
        }
    }
}