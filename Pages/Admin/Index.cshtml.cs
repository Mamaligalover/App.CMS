using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using App.CMS.Data.Context;

namespace App.CMS.Pages.Admin;

[Authorize]
public class IndexModel : PageModel
{
    private readonly AppDbContext _context;
    private readonly ILogger<IndexModel> _logger;

    public int TotalUsers { get; set; }
    public int TotalContent { get; set; }
    public int TotalCategories { get; set; }

    public IndexModel(AppDbContext context, ILogger<IndexModel> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task OnGetAsync()
    {
        TotalUsers = await _context.Users.CountAsync();
        TotalContent = await _context.Contents.CountAsync();
        TotalCategories = await _context.Categories.CountAsync();

        _logger.LogInformation("Admin dashboard accessed by user {User} at {Time}", User.Identity?.Name, DateTime.UtcNow);
    }
}