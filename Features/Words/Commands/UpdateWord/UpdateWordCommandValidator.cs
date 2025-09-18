using FluentValidation;
using Microsoft.EntityFrameworkCore;
using App.CMS.Data.Context;

namespace App.CMS.Features.Words.Commands.UpdateWord;

public class UpdateWordCommandValidator : AbstractValidator<UpdateWordCommand>
{
    private readonly AppDbContext _context;

    public UpdateWordCommandValidator(AppDbContext context)
    {
        _context = context;

        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Id is required")
            .MustAsync(WordExists).WithMessage("Word does not exist");

        RuleFor(x => x.Term)
            .NotEmpty().WithMessage("Term is required")
            .MaximumLength(200).WithMessage("Term must not exceed 200 characters");

        RuleFor(x => x.PartOfSpeech)
            .MaximumLength(50).WithMessage("Part of speech must not exceed 50 characters");

        RuleFor(x => x.CategoryId)
            .NotEmpty().WithMessage("Category is required")
            .MustAsync(CategoryExists).WithMessage("Selected category does not exist");

        RuleFor(x => x)
            .MustAsync(BeUniqueTermInCategory).WithMessage("This term already exists in the selected category");
    }

    private async Task<bool> WordExists(Guid id, CancellationToken cancellationToken)
    {
        return await _context.Words.AnyAsync(w => w.Id == id, cancellationToken);
    }

    private async Task<bool> CategoryExists(Guid categoryId, CancellationToken cancellationToken)
    {
        return await _context.Categories.AnyAsync(c => c.Id == categoryId, cancellationToken);
    }

    private async Task<bool> BeUniqueTermInCategory(UpdateWordCommand command, CancellationToken cancellationToken)
    {
        return !await _context.Words.AnyAsync(w =>
            w.Term == command.Term &&
            w.CategoryId == command.CategoryId &&
            w.Id != command.Id, cancellationToken);
    }
}