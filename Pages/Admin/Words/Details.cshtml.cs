using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MediatR;
using App.CMS.Features.Words.Queries.GetWordById;
using App.CMS.Features.Words.DTOs;

namespace App.CMS.Pages.Admin.Words;

[Authorize]
public class DetailsModel : PageModel
{
    private readonly IMediator _mediator;

    public DetailsModel(IMediator mediator)
    {
        _mediator = mediator;
    }

    public WordDto Word { get; set; } = null!;

    public async Task<IActionResult> OnGetAsync(Guid id)
    {
        var word = await _mediator.Send(new GetWordByIdQuery(id));

        if (word == null)
        {
            TempData["Error"] = "Word not found.";
            return RedirectToPage("Index");
        }

        Word = word;
        return Page();
    }
}