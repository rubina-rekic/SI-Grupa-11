using Microsoft.EntityFrameworkCore;
using PostRoute.DAL.Entities;

namespace PostRoute.DAL.Repositories;

sealed class BoxRepository : IBoxRepository
{
	private readonly AppDbContext _context;

	public BoxRepository(AppDbContext context)
	{
		_context = context;
	}

	public async Task<Box?> GetByIdAsync(Guid boxId, CancellationToken cancellationToken) =>
		await _context.Boxes.FirstOrDefaultAsync(b => b.Id == boxId, cancellationToken);

	public async Task<Box?> GetBySerialNumberAsync(string serialNumber, CancellationToken cancellationToken) =>
		await _context.Boxes.FirstOrDefaultAsync(b => b.SerialNumber == serialNumber, cancellationToken);

	public async Task<bool> SerialNumberExistsAsync(string serialNumber, CancellationToken cancellationToken) =>
		await _context.Boxes.AnyAsync(b => b.SerialNumber == serialNumber, cancellationToken);

	public async Task AddAsync(Box box, CancellationToken cancellationToken)
	{
		await _context.Boxes.AddAsync(box, cancellationToken);
		await _context.SaveChangesAsync(cancellationToken);
	}

	public async Task UpdateAsync(Box box, CancellationToken cancellationToken)
	{
		_context.Boxes.Update(box);
		await _context.SaveChangesAsync(cancellationToken);
	}

	public async Task<IEnumerable<Box>> GetAllAsync(CancellationToken cancellationToken) =>
		await _context.Boxes.ToListAsync(cancellationToken);
}
