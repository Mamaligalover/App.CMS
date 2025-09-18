using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using App.CMS.Data.Context;

namespace App.CMS.Pages;

[Authorize]
public class DashboardModel : PageModel
{
    private readonly AppDbContext _context;
    private readonly ILogger<DashboardModel> _logger;

    public DashboardModel(AppDbContext context, ILogger<DashboardModel> logger)
    {
        _context = context;
        _logger = logger;
    }

    public int UserCount { get; set; }
    public int CategoryCount { get; set; }
    public int MediaCount { get; set; }
    public int WordCount { get; set; }
    public List<RecentContentDto> RecentContent { get; set; } = new();

    public async Task<IActionResult> OnGetAsync()
    {
        try
        {
            // Get statistics
            UserCount = await _context.Users.CountAsync();
            CategoryCount = await _context.Categories.CountAsync();
            MediaCount = await _context.Media.CountAsync();
            WordCount = await _context.Words.CountAsync();

            // Clear recent content since Content entity is removed
            RecentContent = new List<RecentContentDto>();

            return Page();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading dashboard");
            return Page();
        }
    }

    public class RecentContentDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Summary { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string AuthorName { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }
}