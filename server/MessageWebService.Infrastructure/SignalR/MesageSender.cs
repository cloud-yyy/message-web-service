using MessageWebService.Domain.Abstractions;
using MessageWebService.Domain.Models;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;

namespace MessageWebService.Infrastructure.SignalR;

public class MesageSender(
    IHubContext<MessageHub> _hubContext,
    ILogger<MesageSender> _logger
) : IMessageSender
{
    public async Task SendAsync(
        Message message, CancellationToken cancellationToken = default)
    {
        try
        {
            await _hubContext.Clients.All
                .SendAsync("ReceiveMessage", message, cancellationToken);

            _logger.LogInformation(
                "Sent message {sequenceNumber} with text '{text}' at {timestamp}",
                message.SequenceNumber, message.Text, message.Timestamp);
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex, "Error sending message {sequenceNumber} with text '{text}' at {timestamp}",
                message.SequenceNumber, message.Text, message.Timestamp);
            throw;
        }
    }
}
