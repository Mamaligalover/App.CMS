using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using MediatR;
using App.CMS.Features.Words.Commands.UpdateWord;
using App.CMS.Features.Words.Queries.GetWordById;
using App.CMS.Features.Categories.Queries.GetAllCategories;
using System.ComponentModel.DataAnnotations;
using App.CMS.Data.Enums;

namespace App.CMS.Pages.Admin.Words;

[Authorize]
public class EditModel : PageModel
{
    private readonly IMediator _mediator;

    public EditModel(IMediator mediator)
    {
        _mediator = mediator;
    }

    [BindProperty]
    public InputModel Input { get; set; } = new();

    public SelectList Categories { get; set; } = new(new List<object>());

    public class InputModel
    {
        public Guid Id { get; set; }

        [Required]
        [StringLength(200)]
        public string Term { get; set; } = string.Empty;

        [StringLength(50)]
        public string? PartOfSpeech { get; set; }

        [Required]
        public Guid CategoryId { get; set; }

        public WordStatus Status { get; set; }

        public WordDifficulty Difficulty { get; set; }
    }

    public async Task<IActionResult> OnGetAsync(Guid id)
    {
        var word = await _mediator.Send(new GetWordByIdQuery(id));

        if (word == null)
        {
            TempData["Error"] = "Word not found.";
            return RedirectToPage("Index");
        }

        Input = new InputModel
        {
            Id = word.Id,
            Term = word.Term,
            PartOfSpeech = word.PartOfSpeech,
            CategoryId = word.CategoryId,
            Status = word.Status,
            Difficulty = word.Difficulty
        };

        await LoadCategories();
        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            await LoadCategories();
            return Page();
        }

        try
        {
            var command = new UpdateWordCommand
            {
                Id = Input.Id,
                Term = Input.Term,
                PartOfSpeech = Input.PartOfSpeech,
                CategoryId = Input.CategoryId,
                Status = Input.Status,
                Difficulty = Input.Difficulty
            };

            var result = await _mediator.Send(command);

            if (result != null)
            {
                TempData["Success"] = "Word updated successfully.";
                return RedirectToPage("Index");
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Word not found.");
                await LoadCategories();
                return Page();
            }
        }
        catch (Exception)
        {
            ModelState.AddModelError(string.Empty, "An error occurred while updating the word.");
            await LoadCategories();
            return Page();
        }
    }

    private async Task LoadCategories()
    {
        var categories = await _mediator.Send(new GetAllCategoriesQuery());
        Categories = new SelectList(categories, nameof(Features.Categories.DTOs.CategoryDto.Id),
            nameof(Features.Categories.DTOs.CategoryDto.Name));
    }

    private List<string> ParseCommaSeparatedList(string? input)
    {
        if (string.IsNullOrWhiteSpace(input))
            return new List<string>();

        return input.Split(',')
            .Select(s => s.Trim())
            .Where(s => !string.IsNullOrEmpty(s))
            .ToList();
    }
}