using App.CMS.Data.Enums;

namespace App.CMS.Features.Words.DTOs;

public class TranslationDto
{
    public Guid Id { get; set; }
    public Guid WordId { get; set; }
    public Language Language { get; set; }
    public string? TranslatedText { get; set; }
    public string? Pronunciation { get; set; }
    public string? Definition { get; set; }
    public string? UsageExample { get; set; }
    public string? Notes { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}