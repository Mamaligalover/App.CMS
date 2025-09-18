using MediatR;
using Microsoft.EntityFrameworkCore;
using App.CMS.Data.Context;

namespace App.CMS.Features.Words.Commands.DeleteWord;

public class DeleteWordCommandHandler : IRequestHandler<DeleteWordCommand, bool>
{
    private readonly AppDbContext _context;

    public DeleteWordCommandHandler(AppDbContext context)
    {
        _context = context;
    }

    public async Task<bool> Handle(DeleteWordCommand request, CancellationToken cancellationToken)
    {
        var word = await _context.Words
            .FirstOrDefaultAsync(w => w.Id == request.Id, cancellationToken);

        if (word == null)
            return false;

        _context.Words.Remove(word);
        await _context.SaveChangesAsync(cancellationToken);

        return true;
    }
}