using MediatR;
using Microsoft.EntityFrameworkCore;
using App.CMS.Data.Context;

namespace App.CMS.Application.Queries.Users.GetUsers;

public class GetUsersQueryHandler : IRequestHandler<GetUsersQuery, List<UserListDto>>
{
    private readonly AppDbContext _context;

    public GetUsersQueryHandler(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<UserListDto>> Handle(GetUsersQuery request, CancellationToken cancellationToken)
    {
        var query = _context.Users.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(request.SearchTerm))
        {
            query = query.Where(x =>
                x.Username.Contains(request.SearchTerm) ||
                x.Email.Contains(request.SearchTerm) ||
                x.FirstName.Contains(request.SearchTerm) ||
                x.LastName.Contains(request.SearchTerm));
        }

        if (request.IsActive.HasValue)
        {
            query = query.Where(x => x.IsActive == request.IsActive.Value);
        }

        var users = await query
            .OrderByDescending(x => x.CreatedAt)
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(x => new UserListDto
            {
                Id = x.Id,
                Username = x.Username,
                Email = x.Email,
                FullName = $"{x.FirstName} {x.LastName}",
                CreatedAt = x.CreatedAt,
                IsActive = x.IsActive
            })
            .ToListAsync(cancellationToken);

        return users;
    }
}