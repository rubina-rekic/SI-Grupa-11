using PostRoute.BLL.Commands;
using PostRoute.BLL.Models;

namespace PostRoute.BLL.Services;

public interface IBoxService
{
	Task<BoxModel?> GetByIdAsync(Guid boxId, CancellationToken ct);
	Task<BoxModel> CreateAsync(CreateBoxCommand command, CancellationToken ct);
	Task<IEnumerable<BoxModel>> GetAllAsync(CancellationToken ct);
}
