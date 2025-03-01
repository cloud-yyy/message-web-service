using AutoMapper;
using MessageWebService.Domain.Abstractions;
using MessageWebService.Domain.Dtos;
using MessageWebService.Domain.Models;
using MessageWebService.Domain.Requests;
using Microsoft.Extensions.Logging;

namespace MessageWebService.Application.Services;

public class MessageService(
    IMessageRepository _repository,
    IMessageSender _sender,
    IMapper _mapper,
    ILogger<MessageService> _logger) : IMessageService
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

        _logger.LogInformation(
            "Message {sequenceNumber} with text '{text}' at {timestamp} sent successfully",
            message.SequenceNumber, message.Text, message.Timestamp);
    }

    public async Task<IEnumerable<MessageDto>> GetMessagesAsync(
        DateTime start, DateTime end, CancellationToken cancellationToken = default)
    {
        var messages = await _repository
            .GetMessagesAsync(start, end, cancellationToken);

        _logger.LogInformation(
            "Retrieved {count} messages from {start} to {end}",
            messages.Count(), start, end);

        return _mapper.Map<IEnumerable<MessageDto>>(messages);
    }
}
