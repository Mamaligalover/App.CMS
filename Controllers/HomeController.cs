using Microsoft.AspNetCore.Mvc;
using MediatR;
using App.CMS.Application.Queries.Users.GetUsers;
using App.CMS.Models;

namespace App.CMS.Controllers;

public class HomeController : Controller
{
    private readonly IMediator _mediator;
    private readonly ILogger<HomeController> _logger;

    public HomeController(IMediator mediator, ILogger<HomeController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    public async Task<IActionResult> Index()
    {
        var recentContents = await GetRecentContents();

        var model = new HomeViewModel
        {
            Title = "Welcome to App CMS",
            SubTitle = "A Modern Content Management System",
            RecentContents = recentContents
        };

        return View(model);
    }

    public IActionResult About()
    {
        return View();
    }

    public IActionResult Contact()
    {
        return View();
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = HttpContext.TraceIdentifier });
    }

    private async Task<List<ContentSummaryDto>> GetRecentContents()
    {
        // TODO: Replace with actual content query when implemented
        return new List<ContentSummaryDto>
        {
            new ContentSummaryDto
            {
                Title = "Welcome to App CMS",
                Summary = "Get started with your new content management system.",
                PublishedDate = DateTime.UtcNow,
                Author = "Admin",
                Category = "Technology"
            }
        };
    }
}