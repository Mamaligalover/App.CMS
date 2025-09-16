using Microsoft.AspNetCore.Mvc.RazorPages;
using MediatR;

namespace App.CMS.Pages;

public class IndexModel : PageModel
{
    private readonly IMediator _mediator;
    private readonly ILogger<IndexModel> _logger;

    public string Title { get; set; } = "Welcome to App CMS";
    public string SubTitle { get; set; } = "A Modern Content Management System";
    public List<ContentSummaryDto> RecentContents { get; set; } = new();

    public IndexModel(IMediator mediator, ILogger<IndexModel> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    public async Task OnGetAsync()
    {
        // TODO: Replace with actual content query when implemented
        RecentContents = new List<ContentSummaryDto>
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

        await Task.CompletedTask;
    }
}

public class ContentSummaryDto
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Summary { get; set; } = string.Empty;
    public DateTime PublishedDate { get; set; }
    public string Author { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
}