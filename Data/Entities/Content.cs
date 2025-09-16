namespace App.CMS.Data.Entities;

public class Content
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public string Body { get; set; } = string.Empty;
    public string? Summary { get; set; }
    public ContentStatus Status { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public DateTime? PublishedAt { get; set; }

    public Guid AuthorId { get; set; }
    public User Author { get; set; } = null!;

    public Guid? CategoryId { get; set; }
    public Category? Category { get; set; }
}

public enum ContentStatus
{
    Draft,
    Published,
    Archived
}