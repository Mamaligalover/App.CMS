using MediatR;
using App.CMS.Features.Words.DTOs;
using App.CMS.Data.Enums;

namespace App.CMS.Features.Words.Queries.GetAllWords;

public class GetAllWordsQuery : IRequest<List<WordDto>>
{
    public Guid? CategoryId { get; set; }
    public WordStatus? Status { get; set; }
    public bool IncludeAllStatuses { get; set; }
}