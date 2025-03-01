using MessageWebService.Domain.Models;

namespace MessageWebService.Domain.Abstractions;

public interface IMessageRepository
{
    Task SaveMessageAsync(
        Message message, CancellationToken cancellationToken = default);
    Task<IEnumerable<Message>> GetMessagesAsync(
        DateTime start, DateTime end, CancellationToken cancellationToken = default);
}