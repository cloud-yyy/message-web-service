using MessageWebService.Domain.Models;
using MessageWebService.Domain.Requests;

namespace MessageWebService.Domain.Abstractions;

public interface IMessageService
{
    Task SendMessageAsync(
        MessageRequest request, CancellationToken cancellationToken = default);
    Task<List<Message>> GetMessagesAsync(
        DateTime start, DateTime end, CancellationToken cancellationToken = default);
}
