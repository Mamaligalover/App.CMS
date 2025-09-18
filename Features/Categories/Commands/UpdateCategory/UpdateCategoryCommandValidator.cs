using FluentValidation;
using Microsoft.EntityFrameworkCore;
using App.CMS.Data.Context;

namespace App.CMS.Features.Categories.Commands.UpdateCategory;

public class UpdateCategoryCommandValidator : AbstractValidator<UpdateCategoryCommand>
{
    private readonly AppDbContext _context;

    public UpdateCategoryCommandValidator(AppDbContext context)
    {
        _context = context;

        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Id is required")
            .MustAsync(CategoryExists).WithMessage("Category does not exist");

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
            .When(x => x.ParentCategoryId.HasValue)
            .MustAsync(NotSelfReference).WithMessage("Category cannot be its own parent")
            .MustAsync(NotCircularReference).WithMessage("This would create a circular reference");
    }

    private async Task<bool> CategoryExists(Guid id, CancellationToken cancellationToken)
    {
        return await _context.Categories.AnyAsync(c => c.Id == id, cancellationToken);
    }

    private async Task<bool> BeUniqueSlug(UpdateCategoryCommand command, string slug, CancellationToken cancellationToken)
    {
        return !await _context.Categories.AnyAsync(c => c.Slug == slug && c.Id != command.Id, cancellationToken);
    }

    private async Task<bool> ParentCategoryExists(Guid? parentId, CancellationToken cancellationToken)
    {
        if (!parentId.HasValue) return true;
        return await _context.Categories.AnyAsync(c => c.Id == parentId.Value, cancellationToken);
    }

    private Task<bool> NotSelfReference(UpdateCategoryCommand command, Guid? parentId, CancellationToken cancellationToken)
    {
        return Task.FromResult(!parentId.HasValue || parentId.Value != command.Id);
    }

    private async Task<bool> NotCircularReference(UpdateCategoryCommand command, Guid? parentId, CancellationToken cancellationToken)
    {
        if (!parentId.HasValue) return true;

        var visited = new HashSet<Guid>();
        var currentId = parentId.Value;

        while (currentId != Guid.Empty)
        {
            if (visited.Contains(currentId) || currentId == command.Id)
                return false;

            visited.Add(currentId);

            var parent = await _context.Categories
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.Id == currentId, cancellationToken);

            if (parent?.ParentCategoryId == null)
                break;

            currentId = parent.ParentCategoryId.Value;
        }

        return true;
    }
}