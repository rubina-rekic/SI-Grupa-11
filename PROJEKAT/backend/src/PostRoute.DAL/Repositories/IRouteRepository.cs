using PostRoute.DAL.Entities;

namespace PostRoute.DAL.Repositories;

public interface IRouteRepository
{
    Task<Route> CreateAsync(Route route, CancellationToken cancellationToken = default);
    Task<Route?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Route?> GetByPostmanAndDateAsync(Guid postmanId, DateOnly date, CancellationToken cancellationToken = default);
    Task<Route?> GetActiveByPostmanAndMailboxAsync(Guid postmanId, Guid mailboxId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Guid>> GetPostmanIdsWithActiveRouteOnDateAsync(
        DateOnly date,
        Guid? excludedRouteId = null,
        CancellationToken cancellationToken = default);
    Task<Dictionary<Guid, DateOnly>> GetLastIncludedDatesByMailboxIdsAsync(
        IEnumerable<Guid> mailboxIds,
        DateOnly upToDate,
        CancellationToken cancellationToken = default);

    Task<Route> UpdateAsync(Route route, CancellationToken cancellationToken = default);
    Task<List<Route>> GetByDateAsync(DateOnly date, CancellationToken cancellationToken = default);

    Task<(IReadOnlyList<Route> Items, int TotalCount)> GetPagedArchiveAsync(
        int page,
        int pageSize,
        DateOnly? fromDate,
        DateOnly? toDate,
        Guid? postmanId,
        CancellationToken cancellationToken = default);
}
