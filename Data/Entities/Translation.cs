using System.ComponentModel.DataAnnotations;
using App.CMS.Data.Enums;

namespace App.CMS.Data.Entities;

public class Translation
{
    public Guid Id { get; set; }

    [Required]
    public Guid WordId { get; set; }
    public Word Word { get; set; } = null!;

    [Required]
    public Language Language { get; set; }

    [MaxLength(500)]
    public string? TranslatedText { get; set; }

    [MaxLength(200)]
    public string? Pronunciation { get; set; }

    [MaxLength(1500)]
    public string? Definition { get; set; }

    [MaxLength(1000)]
    public string? UsageExample { get; set; }

    [MaxLength(1000)]
    public string? Notes { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime? UpdatedAt { get; set; }
}