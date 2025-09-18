using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using MediatR;
using App.CMS.Features.Words.Commands.CreateWord;
using App.CMS.Features.Categories.Queries.GetAllCategories;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using App.CMS.Data.Enums;

namespace App.CMS.Pages.Admin.Words;

[Authorize]
public class CreateModel : PageModel
{
    private readonly IMediator _mediator;

    public CreateModel(IMediator mediator)
    {
        _mediator = mediator;
    }

    [BindProperty]
    public InputModel Input { get; set; } = new();

    public SelectList Categories { get; set; } = new(new List<object>());

    public class InputModel
    {
        [Required]
        [StringLength(200)]
        public string Term { get; set; } = string.Empty;

        [StringLength(50)]
        public string? PartOfSpeech { get; set; }

        [Required]
        public Guid CategoryId { get; set; }

        public WordStatus Status { get; set; } = WordStatus.Draft;

        public WordDifficulty Difficulty { get; set; } = WordDifficulty.Medium;
    }

    public async Task OnGetAsync()
    {
        await LoadCategories();
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
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            var command = new CreateWordCommand
            {
                Term = Input.Term,
                PartOfSpeech = Input.PartOfSpeech,
                CategoryId = Input.CategoryId,
                Status = Input.Status,
                Difficulty = Input.Difficulty,
                CreatedByUserId = string.IsNullOrEmpty(userId) ? null : Guid.Parse(userId)
            };

            await _mediator.Send(command);
            TempData["Success"] = "Word created successfully.";
            return RedirectToPage("Index");
        }
        catch (Exception)
        {
            ModelState.AddModelError(string.Empty, "An error occurred while creating the word.");
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