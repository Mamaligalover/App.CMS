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
using App.CMS.Features.Words.DTOs;

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

        public List<TranslationInputModel> Translations { get; set; } = new();
    }

    public class TranslationInputModel
    {
        public Language Language { get; set; }

        [StringLength(500)]
        public string? TranslatedText { get; set; }

        [StringLength(200)]
        public string? Pronunciation { get; set; }

        [StringLength(1500)]
        public string? Definition { get; set; }

        [StringLength(1000)]
        public string? UsageExample { get; set; }

        [StringLength(1000)]
        public string? Notes { get; set; }
    }

    public async Task OnGetAsync()
    {
        await LoadCategories();
        InitializeTranslations();
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
                CreatedByUserId = string.IsNullOrEmpty(userId) ? null : Guid.Parse(userId),
                Translations = Input.Translations
                    .Where(t => !string.IsNullOrWhiteSpace(t.TranslatedText) || !string.IsNullOrWhiteSpace(t.Definition))
                    .Select(t => new TranslationDto
                    {
                        Language = t.Language,
                        TranslatedText = t.TranslatedText,
                        Pronunciation = t.Pronunciation,
                        Definition = t.Definition,
                        UsageExample = t.UsageExample,
                        Notes = t.Notes
                    }).ToList()
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

    private void InitializeTranslations()
    {
        var languages = Enum.GetValues<Language>();

        foreach (var language in languages)
        {
            Input.Translations.Add(new TranslationInputModel
            {
                Language = language,
                TranslatedText = string.Empty,
                Pronunciation = string.Empty,
                Definition = string.Empty,
                UsageExample = string.Empty,
                Notes = string.Empty
            });
        }
    }

    public string GetLanguageDisplayName(Language language)
    {
        return language.ToString();
    }
}