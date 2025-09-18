using MediatR;

namespace App.CMS.Features.Categories.Commands.DeleteCategory;

public class DeleteCategoryCommand : IRequest<bool>
{
    public Guid Id { get; set; }

    public DeleteCategoryCommand(Guid id)
    {
        Id = id;
    }
}