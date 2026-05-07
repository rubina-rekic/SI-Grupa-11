using PostRoute.BLL.Commands;
using PostRoute.DAL.Entities;
using PostRoute.DAL.Repositories;

namespace PostRoute.BLL.Services;

public class MailboxService : IMailboxService
{
    private readonly IMailboxRepository _mailboxRepository;

    public MailboxService(IMailboxRepository mailboxRepository)
    {
        _mailboxRepository = mailboxRepository;
    }

    public async Task<Mailbox> CreateAsync(CreateMailboxCommand command, CancellationToken cancellationToken)
    {
        // Validate unique serial number
        if (await _mailboxRepository.SerialNumberExistsAsync(command.SerialNumber, cancellationToken))
        {
            throw new InvalidOperationException($"Sandučić sa serijskim brojem '{command.SerialNumber}' već postoji.");
        }

        // Validate coordinates
        if (command.Latitude < -90 || command.Latitude > 90)
        {
            throw new InvalidOperationException("Latitude mora biti između -90 i 90.");
        }

        if (command.Longitude < -180 || command.Longitude > 180)
        {
            throw new InvalidOperationException("Longitude mora biti između -180 i 180.");
        }

        // Validate capacity
        if (command.Capacity <= 0)
        {
            throw new InvalidOperationException("Kapacitet mora biti veći od 0.");
        }

        // Validate installation year
        var currentYear = DateTime.Now.Year;
        if (command.InstallationYear < 1900 || command.InstallationYear > currentYear + 10)
        {
            throw new InvalidOperationException($"Godina instalacije mora biti između 1900 i {currentYear + 10}.");
        }

        var mailbox = new Mailbox
        {
            Id = Guid.NewGuid(),
            SerialNumber = command.SerialNumber.Trim(),
            Address = command.Address.Trim(),
            Latitude = command.Latitude,
            Longitude = command.Longitude,
            Type = command.Type,
            Capacity = command.Capacity,
            InstallationYear = command.InstallationYear,
            Notes = command.Notes?.Trim()
        };

        return await _mailboxRepository.AddAsync(mailbox, cancellationToken);
    }

    public async Task<Mailbox?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return await _mailboxRepository.GetByIdAsync(id, cancellationToken);
    }

    public async Task<Mailbox?> GetBySerialNumberAsync(string serialNumber, CancellationToken cancellationToken)
    {
        return await _mailboxRepository.GetBySerialNumberAsync(serialNumber, cancellationToken);
    }

    public async Task<IEnumerable<Mailbox>> GetAllAsync(CancellationToken cancellationToken)
    {
        return await _mailboxRepository.GetAllAsync(cancellationToken);
    }

    public async Task<bool> SerialNumberExistsAsync(string serialNumber, CancellationToken cancellationToken)
    {
        return await _mailboxRepository.SerialNumberExistsAsync(serialNumber, cancellationToken);
    }
}
