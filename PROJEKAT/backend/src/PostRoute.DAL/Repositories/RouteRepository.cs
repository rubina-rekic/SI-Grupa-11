using PostRoute.DAL.Entities;
using Microsoft.EntityFrameworkCore;

namespace PostRoute.DAL.Repositories;

public class RouteRepository : IRouteRepository
{
    private readonly AppDbContext _context;

    public RouteRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Route> CreateAsync(Route route, CancellationToken cancellationToken = default)
    {
        _context.Routes.Add(route);
        await _context.SaveChangesAsync(cancellationToken);
        return route;
    }

    public async Task<Route?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Routes
            .Include(r => r.RouteItems)
                .ThenInclude(ri => ri.Mailbox)
            .Include(r => r.Postman)
            .FirstOrDefaultAsync(r => r.Id == id, cancellationToken);
    }

    public async Task<Route?> GetByPostmanAndDateAsync(Guid postmanId, DateOnly date, CancellationToken cancellationToken = default)
    {
        return await _context.Routes
            .Include(r => r.RouteItems)
                .ThenInclude(ri => ri.Mailbox)
            .FirstOrDefaultAsync(r => r.PostmanId == postmanId && r.Date == date, cancellationToken);
    }

    public async Task<Dictionary<Guid, DateOnly>> GetLastIncludedDatesByMailboxIdsAsync(
        IEnumerable<Guid> mailboxIds,
        DateOnly upToDate,
        CancellationToken cancellationToken = default)
    {
        var mailboxIdSet = mailboxIds.Distinct().ToList();
        if (mailboxIdSet.Count == 0)
        {
            return new Dictionary<Guid, DateOnly>();
        }

        return await _context.RouteItems
            .Where(ri => mailboxIdSet.Contains(ri.MailboxId) && ri.Route.Date <= upToDate)
            .GroupBy(ri => ri.MailboxId)
            .Select(g => new
            {
                MailboxId = g.Key,
                LastDate = g.Max(x => x.Route.Date)
            })
            .ToDictionaryAsync(x => x.MailboxId, x => x.LastDate, cancellationToken);
    }
}
