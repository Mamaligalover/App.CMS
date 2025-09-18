using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MediatR;
using App.CMS.Features.Words.Queries.GetWordById;
using App.CMS.Features.Words.Commands.UpdateWord;
using App.CMS.Features.Words.DTOs;
using App.CMS.Data.Enums;
using System.ComponentModel.DataAnnotations;

namespace App.CMS.Pages.Admin.Words;

[Authorize]
public class TranslationsModel : PageModel
{
    private readonly IMediator _mediator;

    public TranslationsModel(IMediator mediator)
    {
        _mediator = mediator;
    }

    [BindProperty]
    public Guid WordId { get; set; }

    [BindProperty]
    public InputModel Input { get; set; } = new();

    public WordDto Word { get; set; } = null!;

    public class InputModel
    {
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

    public async Task<IActionResult> OnGetAsync(Guid id)
    {
        WordId = id;
        Word = await _mediator.Send(new GetWordByIdQuery(id));

        if (Word == null)
        {
            TempData["Error"] = "Word not found.";
            return RedirectToPage("Index");
        }

        InitializeTranslations();
        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            Word = await _mediator.Send(new GetWordByIdQuery(WordId));
            return Page();
        }

        var word = await _mediator.Send(new GetWordByIdQuery(WordId));
        if (word == null)
        {
            TempData["Error"] = "Word not found.";
            return RedirectToPage("Index");
        }

        var command = new UpdateWordCommand
        {
            Id = WordId,
            Term = word.Term,
            PartOfSpeech = word.PartOfSpeech,
            CategoryId = word.CategoryId,
            Status = word.Status,
            Difficulty = word.Difficulty,
            ImageId = word.ImageId,
            AudioId = word.AudioId,
            VideoId = word.VideoId,
            Translations = Input.Translations
                .Where(t => !string.IsNullOrWhiteSpace(t.TranslatedText))
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
        TempData["Success"] = "Translations updated successfully.";
        return RedirectToPage("Details", new { id = WordId });
    }

    private void InitializeTranslations()
    {
        var languages = Enum.GetValues<Language>();

        foreach (var language in languages)
        {
            var existingTranslation = Word.Translations?.FirstOrDefault(t => t.Language == language);

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