using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using MediatR;
using App.CMS.Features.Words.Queries.GetAllWords;
using App.CMS.Features.Words.Commands.DeleteWord;
using App.CMS.Features.Words.DTOs;
using App.CMS.Features.Categories.Queries.GetAllCategories;

namespace App.CMS.Pages.Admin.Words;

[Authorize]
public class IndexModel : PageModel
{
    private readonly IMediator _mediator;

    public IndexModel(IMediator mediator)
    {
        _mediator = mediator;
    }

    public List<WordDto> Words { get; set; } = new();
    public SelectList Categories { get; set; } = new(new List<object>());

    [BindProperty(SupportsGet = true)]
    public Guid? CategoryId { get; set; }

    public async Task OnGetAsync()
    {
        var categories = await _mediator.Send(new GetAllCategoriesQuery());
        Categories = new SelectList(categories, nameof(Features.Categories.DTOs.CategoryDto.Id),
            nameof(Features.Categories.DTOs.CategoryDto.Name), CategoryId);

        Words = await _mediator.Send(new GetAllWordsQuery
        {
            CategoryId = CategoryId,
            IncludeAllStatuses = true
        });
    }

    public async Task<IActionResult> OnPostDeleteAsync(Guid id)
    {
        var result = await _mediator.Send(new DeleteWordCommand(id));

        if (result)
        {
            TempData["Success"] = "Word deleted successfully.";
        }
        else
        {
            TempData["Error"] = "Word not found.";
        }

        return RedirectToPage();
    }
}