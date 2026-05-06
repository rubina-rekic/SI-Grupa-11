using PostRoute.DAL.Entities;

namespace PostRoute.DAL.Repositories;

public interface IBoxRepository
{
	Task<Box?> GetByIdAsync(Guid boxId, CancellationToken cancellationToken);
	Task<Box?> GetBySerialNumberAsync(string serialNumber, CancellationToken cancellationToken);
	Task<bool> SerialNumberExistsAsync(string serialNumber, CancellationToken cancellationToken);
	Task AddAsync(Box box, CancellationToken cancellationToken);
	Task UpdateAsync(Box box, CancellationToken cancellationToken);
	Task<IEnumerable<Box>> GetAllAsync(CancellationToken cancellationToken);
}
