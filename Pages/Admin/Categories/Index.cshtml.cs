using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MediatR;
using App.CMS.Features.Categories.Queries.GetAllCategories;
using App.CMS.Features.Categories.Commands.DeleteCategory;
using App.CMS.Features.Categories.DTOs;

namespace App.CMS.Pages.Admin.Categories;

[Authorize]
public class IndexModel : PageModel
{
    private readonly IMediator _mediator;

    public IndexModel(IMediator mediator)
    {
        _mediator = mediator;
    }

    public List<CategoryDto> Categories { get; set; } = new();

    public async Task OnGetAsync()
    {
        Categories = await _mediator.Send(new GetAllCategoriesQuery { IncludeInactive = true });
    }

    public async Task<IActionResult> OnPostDeleteAsync(Guid id)
    {
        var result = await _mediator.Send(new DeleteCategoryCommand(id));

        if (result)
        {
            TempData["Success"] = "Category deleted successfully.";
        }
        else
        {
            TempData["Error"] = "Category not found.";
        }

        return RedirectToPage();
    }
}