using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using App.CMS.Data.Context;

namespace App.CMS.Controllers;

[ApiController]
[Route("[controller]")]
public class HealthController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly IConfiguration _configuration;
    private readonly ILogger<HealthController> _logger;

    public HealthController(
        AppDbContext context,
        IConfiguration configuration,
        ILogger<HealthController> logger)
    {
        _context = context;
        _configuration = configuration;
        _logger = logger;
    }

    [HttpGet]
    [Route("/health")]
    public async Task<IActionResult> Health()
    {
        try
        {
            // Check database connectivity
            await _context.Database.CanConnectAsync();

            return Ok(new
            {
                status = "Healthy",
                timestamp = DateTime.UtcNow,
                services = new
                {
                    database = "Connected",
                    application = "Running"
                }
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Health check failed");

            return StatusCode(503, new
            {
                status = "Unhealthy",
                timestamp = DateTime.UtcNow,
                error = "Database connection failed"
            });
        }
    }

    [HttpGet]
    [Route("/health/ready")]
    public async Task<IActionResult> Ready()
    {
        try
        {
            // More comprehensive readiness check
            await _context.Database.CanConnectAsync();

            // Check if migrations are applied
            var pendingMigrations = await _context.Database.GetPendingMigrationsAsync();
            if (pendingMigrations.Any())
            {
                return StatusCode(503, new
                {
                    status = "Not Ready",
                    timestamp = DateTime.UtcNow,
                    reason = "Database migrations pending"
                });
            }

            return Ok(new
            {
                status = "Ready",
                timestamp = DateTime.UtcNow
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Readiness check failed");

            return StatusCode(503, new
            {
                status = "Not Ready",
                timestamp = DateTime.UtcNow,
                error = ex.Message
            });
        }
    }

    [HttpGet]
    [Route("/health/live")]
    public IActionResult Live()
    {
        // Simple liveness check - if the app can respond, it's alive
        return Ok(new
        {
            status = "Alive",
            timestamp = DateTime.UtcNow
        });
    }
}