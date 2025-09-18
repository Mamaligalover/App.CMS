using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using MediatR;
using App.CMS.Features.Categories.Commands.CreateCategory;
using App.CMS.Features.Categories.Queries.GetAllCategories;
using System.ComponentModel.DataAnnotations;

namespace App.CMS.Pages.Admin.Categories;

[Authorize]
public class CreateModel : PageModel
{
    private readonly IMediator _mediator;

    public CreateModel(IMediator mediator)
    {
        _mediator = mediator;
    }

    [BindProperty]
    public InputModel Input { get; set; } = new();

    public SelectList ParentCategories { get; set; } = new(new List<object>());

    public class InputModel
    {
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

        public bool IsActive { get; set; } = true;
    }

    public async Task OnGetAsync()
    {
        await LoadParentCategories();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            await LoadParentCategories();
            return Page();
        }

        try
        {
            var command = new CreateCategoryCommand
            {
                Name = Input.Name,
                Slug = Input.Slug,
                Description = Input.Description,
                ParentCategoryId = Input.ParentCategoryId,
                IsActive = Input.IsActive
            };

            await _mediator.Send(command);
            TempData["Success"] = "Category created successfully.";
            return RedirectToPage("Index");
        }
        catch (Exception)
        {
            ModelState.AddModelError(string.Empty, "An error occurred while creating the category.");
            await LoadParentCategories();
            return Page();
        }
    }

    private async Task LoadParentCategories()
    {
        var categories = await _mediator.Send(new GetAllCategoriesQuery());
        ParentCategories = new SelectList(categories, nameof(Features.Categories.DTOs.CategoryDto.Id),
            nameof(Features.Categories.DTOs.CategoryDto.Name));
    }
}