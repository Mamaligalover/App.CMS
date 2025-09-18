using MediatR;
using App.CMS.Features.Words.DTOs;

namespace App.CMS.Features.Words.Queries.GetWordById;

public class GetWordByIdQuery : IRequest<WordDto?>
{
    public Guid Id { get; set; }

    public GetWordByIdQuery(Guid id)
    {
        Id = id;
    }
}