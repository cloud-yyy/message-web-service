using MessageWebService.Domain.Abstractions;
using MessageWebService.Domain.Requests;
using Microsoft.AspNetCore.Mvc;

namespace MessageWebService.Presentation.Controllers;

[ApiController]
[Route("api/messages")]
public class MessageController(
    IMessageService _messageService) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> PostMessage(
        [FromBody] MessageRequest request,
        CancellationToken cancellationToken)
    {
        await _messageService.SendMessageAsync(request, cancellationToken);
        return Ok();
    }

    [HttpGet]
    public async Task<IActionResult> GetMessages(
        [FromQuery] DateTime? start,
        [FromQuery] DateTime? end,
        CancellationToken cancellationToken)
    {
        var messages = await _messageService.GetMessagesAsync(
            start ?? DateTime.UtcNow.AddMinutes(-10),
            end ?? DateTime.UtcNow, cancellationToken);
        
        return Ok(messages);
    }
}
