using FluentValidation;
using Microsoft.EntityFrameworkCore;
using App.CMS.Data.Context;

namespace App.CMS.Features.Categories.Commands.CreateCategory;

public class CreateCategoryCommandValidator : AbstractValidator<CreateCategoryCommand>
{
    private readonly AppDbContext _context;

    public CreateCategoryCommandValidator(AppDbContext context)
    {
        _context = context;

        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name is required")
            .MaximumLength(100).WithMessage("Name must not exceed 100 characters");

        RuleFor(x => x.Slug)
            .NotEmpty().WithMessage("Slug is required")
            .MaximumLength(100).WithMessage("Slug must not exceed 100 characters")
            .Matches(@"^[a-z0-9]+(?:-[a-z0-9]+)*$").WithMessage("Slug must be lowercase with hyphens only")
            .MustAsync(BeUniqueSlug).WithMessage("Slug already exists");

        RuleFor(x => x.Description)
            .MaximumLength(500).WithMessage("Description must not exceed 500 characters");

        RuleFor(x => x.ParentCategoryId)
            .MustAsync(ParentCategoryExists).WithMessage("Parent category does not exist")
            .When(x => x.ParentCategoryId.HasValue);
    }

    private async Task<bool> BeUniqueSlug(string slug, CancellationToken cancellationToken)
    {
        return !await _context.Categories.AnyAsync(c => c.Slug == slug, cancellationToken);
    }

    private async Task<bool> ParentCategoryExists(Guid? parentId, CancellationToken cancellationToken)
    {
        if (!parentId.HasValue) return true;
        return await _context.Categories.AnyAsync(c => c.Id == parentId.Value, cancellationToken);
    }
}