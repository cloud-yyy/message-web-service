using MessageWebService.Domain.Dtos;
using MessageWebService.Domain.Requests;

namespace MessageWebService.Domain.Abstractions;

public interface IMessageService
{
    Task SendMessageAsync(
        MessageRequest request, CancellationToken cancellationToken = default);
    Task<IEnumerable<MessageDto>> GetMessagesAsync(
        DateTime start, DateTime end, CancellationToken cancellationToken = default);
}
