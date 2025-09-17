namespace App.CMS.Data.Entities;

public class Category
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public string? Description { get; set; }
    public Guid? ParentCategoryId { get; set; }
    public Category? ParentCategory { get; set; }
    public int Level { get; set; } = 0;
    public string Path { get; set; } = string.Empty;
    public int DisplayOrder { get; set; } = 0;
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }

    public Guid? IconId { get; set; }
    public Media? Icon { get; set; }

    public ICollection<Category> SubCategories { get; set; } = new List<Category>();
}