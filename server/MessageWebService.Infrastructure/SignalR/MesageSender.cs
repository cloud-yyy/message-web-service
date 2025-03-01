using MessageWebService.Domain.Abstractions;
using MessageWebService.Domain.Models;
using Microsoft.AspNetCore.SignalR;

namespace MessageWebService.Infrastructure.SignalR;

public class MesageSender(
    IHubContext<MessageHub> _hubContext
) : IMessageSender
{
    public async Task SendAsync(
        Message message, CancellationToken cancellationToken = default)
    {
        await _hubContext.Clients.All
            .SendAsync("ReceiveMessage", message, cancellationToken);
    }
}
