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
            .Include(r => r.Postman)
            .FirstOrDefaultAsync(r => r.PostmanId == postmanId && r.Date == date, cancellationToken);
    }

    public async Task<Route?> GetActiveByPostmanAndMailboxAsync(Guid postmanId, Guid mailboxId, CancellationToken cancellationToken = default)
    {
        return await _context.Routes
            .Include(r => r.RouteItems)
                .ThenInclude(ri => ri.Mailbox)
            .Include(r => r.Postman)
            .Where(r => r.PostmanId == postmanId)
            .Where(r => r.Status == RouteStatus.Dodijeljena || r.Status == RouteStatus.UProgresu)
            .Where(r => r.RouteItems.Any(ri => ri.MailboxId == mailboxId))
            .OrderByDescending(r => r.Date)
            .ThenByDescending(r => r.PlannedStartTime)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Guid>> GetPostmanIdsWithActiveRouteOnDateAsync(
        DateOnly date,
        Guid? excludedRouteId = null,
        CancellationToken cancellationToken = default)
    {
        return await _context.Routes
            .Where(r => r.Date == date)
            .Where(r => excludedRouteId == null || r.Id != excludedRouteId.Value)
            .Where(r => r.Status == RouteStatus.Dodijeljena || r.Status == RouteStatus.UProgresu)
            .Select(r => r.PostmanId)
            .Distinct()
            .ToListAsync(cancellationToken);
    }

    public async Task<Route> UpdateAsync(Route route, CancellationToken cancellationToken = default)
    {
        _context.Routes.Update(route);
        await _context.SaveChangesAsync(cancellationToken);
        return route;
    }

    public async Task<List<Route>> GetByDateAsync(DateOnly date, CancellationToken cancellationToken = default)
    {
        return await _context.Routes
            .Include(r => r.RouteItems)
                .ThenInclude(ri => ri.Mailbox)
            .Include(r => r.Postman)
            .Where(r => r.Date == date)
            .OrderBy(r => r.Status)
            .ThenBy(r => r.PlannedStartTime)
            .ToListAsync(cancellationToken);
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

    public async Task<(IReadOnlyList<Route> Items, int TotalCount)> GetPagedArchiveAsync(
        int page,
        int pageSize,
        DateOnly? fromDate,
        DateOnly? toDate,
        Guid? postmanId,
        CancellationToken cancellationToken = default)
    {
        var query = _context.Routes
            .Include(r => r.Postman)
            .Include(r => r.RouteItems)
                .ThenInclude(ri => ri.Mailbox)
            .Where(r => r.Status == RouteStatus.Zavrsena || r.Status == RouteStatus.Otkazana)
            .AsQueryable();

        if (fromDate.HasValue)
            query = query.Where(r => r.Date >= fromDate.Value);

        if (toDate.HasValue)
            query = query.Where(r => r.Date <= toDate.Value);

        if (postmanId.HasValue)
            query = query.Where(r => r.PostmanId == postmanId.Value);

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderByDescending(r => r.Date)
            .ThenByDescending(r => r.PlannedStartTime)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (items, totalCount);
    }
}
