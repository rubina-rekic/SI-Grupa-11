using PostRoute.DAL.Entities;

namespace PostRoute.DAL.Repositories;

public interface ISecurityLogRepository
{
    Task AddAsync(SecurityLog securityLog, CancellationToken cancellationToken);
    Task<List<SecurityLog>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken);
    Task<List<SecurityLog>> GetUnauthorizedAttemptsAsync(CancellationToken cancellationToken);
}
