using MessageWebService.Domain.Models;

namespace MessageWebService.Domain.Abstractions;

public interface IMessageSender
{
    Task SendAsync(Message message, CancellationToken cancellationToken = default);
}
