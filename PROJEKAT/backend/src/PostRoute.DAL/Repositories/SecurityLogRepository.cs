using Microsoft.EntityFrameworkCore;
using PostRoute.DAL.Entities;

namespace PostRoute.DAL.Repositories;

public sealed class SecurityLogRepository : ISecurityLogRepository
{
    private readonly AppDbContext _context;

    public SecurityLogRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(SecurityLog securityLog, CancellationToken cancellationToken)
    {
        await _context.SecurityLogs.AddAsync(securityLog, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task<List<SecurityLog>> GetByUserIdAsync(string userId, CancellationToken cancellationToken)
    {
        return await _context.SecurityLogs
            .Where(s => s.UserId == userId)
            .OrderByDescending(s => s.Timestamp)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<SecurityLog>> GetUnauthorizedAttemptsAsync(CancellationToken cancellationToken)
    {
        return await _context.SecurityLogs
            .Where(s => !s.IsSuccessful)
            .OrderByDescending(s => s.Timestamp)
            .ToListAsync(cancellationToken);
    }
}
