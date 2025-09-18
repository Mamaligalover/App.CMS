using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using MediatR;
using App.CMS.Features.Categories.Commands.UpdateCategory;
using App.CMS.Features.Categories.Queries.GetAllCategories;
using App.CMS.Features.Categories.Queries.GetCategoryById;
using System.ComponentModel.DataAnnotations;

namespace App.CMS.Pages.Admin.Categories;

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

    public SelectList ParentCategories { get; set; } = new(new List<object>());

    public class InputModel
    {
        public Guid Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        [RegularExpression(@"^[a-z0-9]+(?:-[a-z0-9]+)*$", ErrorMessage = "Slug must be lowercase with hyphens only")]
        public string Slug { get; set; } = string.Empty;

        [StringLength(500)]
        public string? Description { get; set; }

        public Guid? ParentCategoryId { get; set; }

        public bool IsActive { get; set; }
    }

    public async Task<IActionResult> OnGetAsync(Guid id)
    {
        var category = await _mediator.Send(new GetCategoryByIdQuery(id));

        if (category == null)
        {
            TempData["Error"] = "Category not found.";
            return RedirectToPage("Index");
        }

        Input = new InputModel
        {
            Id = category.Id,
            Name = category.Name,
            Slug = category.Slug,
            Description = category.Description,
            ParentCategoryId = category.ParentCategoryId,
            IsActive = category.IsActive
        };

        await LoadParentCategories(id);
        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            await LoadParentCategories(Input.Id);
            return Page();
        }

        try
        {
            var command = new UpdateCategoryCommand
            {
                Id = Input.Id,
                Name = Input.Name,
                Slug = Input.Slug,
                Description = Input.Description,
                ParentCategoryId = Input.ParentCategoryId,
                IsActive = Input.IsActive
            };

            var result = await _mediator.Send(command);

            if (result != null)
            {
                TempData["Success"] = "Category updated successfully.";
                return RedirectToPage("Index");
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Category not found.");
                await LoadParentCategories(Input.Id);
                return Page();
            }
        }
        catch (Exception)
        {
            ModelState.AddModelError(string.Empty, "An error occurred while updating the category.");
            await LoadParentCategories(Input.Id);
            return Page();
        }
    }

    private async Task LoadParentCategories(Guid currentCategoryId)
    {
        var categories = await _mediator.Send(new GetAllCategoriesQuery());
        // Exclude current category and its descendants to prevent circular references
        var availableParents = categories.Where(c => c.Id != currentCategoryId).ToList();
        ParentCategories = new SelectList(availableParents, nameof(Features.Categories.DTOs.CategoryDto.Id),
            nameof(Features.Categories.DTOs.CategoryDto.Name));
    }
}