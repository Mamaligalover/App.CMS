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
using App.CMS.Features.Words.DTOs;

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

        public List<TranslationInputModel> Translations { get; set; } = new();
    }

    public class TranslationInputModel
    {
        public Language Language { get; set; }

        [StringLength(500)]
        public string TranslatedText { get; set; } = string.Empty;

        [StringLength(200)]
        public string? Pronunciation { get; set; }

        [StringLength(1500)]
        public string Definition { get; set; } = string.Empty;

        [StringLength(1000)]
        public string? UsageExample { get; set; }

        [StringLength(1000)]
        public string? Notes { get; set; }
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

        InitializeTranslations(word);
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
                Difficulty = Input.Difficulty,
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

    private void InitializeTranslations(WordDto word)
    {
        var languages = Enum.GetValues<Language>();

        foreach (var language in languages)
        {
            var existingTranslation = word.Translations?.FirstOrDefault(t => t.Language == language);

            Input.Translations.Add(new TranslationInputModel
            {
                Language = language,
                TranslatedText = existingTranslation?.TranslatedText ?? string.Empty,
                Pronunciation = existingTranslation?.Pronunciation,
                Definition = existingTranslation?.Definition ?? string.Empty,
                UsageExample = existingTranslation?.UsageExample,
                Notes = existingTranslation?.Notes
            });
        }
    }

    public string GetLanguageDisplayName(Language language)
    {
        return language.ToString();
    }
}