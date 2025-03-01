using AutoMapper;
using MessageWebService.Domain.Abstractions;
using MessageWebService.Domain.Dtos;
using MessageWebService.Domain.Models;
using MessageWebService.Domain.Requests;

namespace MessageWebService.Application.Services;

public class MessageService(
    IMessageRepository _repository,
    IMessageSender _sender,
    IMapper _mapper) : IMessageService
{
    public async Task SendMessageAsync(
        MessageRequest request, CancellationToken cancellationToken = default)
    {
        var message = new Message
        {
            Text = request.Text,
            Timestamp = DateTime.UtcNow,
            SequenceNumber = request.SequenceNumber
        };

        await _repository.SaveMessageAsync(message, cancellationToken);
        await _sender.SendAsync(message, cancellationToken);
    }

    public async Task<IEnumerable<MessageDto>> GetMessagesAsync(
        DateTime start, DateTime end, CancellationToken cancellationToken = default)
    {
        var messages =  await _repository
            .GetMessagesAsync(start, end, cancellationToken);
        
        return _mapper.Map<List<MessageDto>>(messages);
    }
}
