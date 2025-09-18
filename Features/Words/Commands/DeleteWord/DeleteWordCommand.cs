using MediatR;

namespace App.CMS.Features.Words.Commands.DeleteWord;

public class DeleteWordCommand : IRequest<bool>
{
    public Guid Id { get; set; }

    public DeleteWordCommand(Guid id)
    {
        Id = id;
    }
}