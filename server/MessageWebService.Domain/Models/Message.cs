namespace MessageWebService.Domain.Models;

public class Message
{
    public int Id { get; set; }
    public string Text { get; set; } = null!;
    public DateTime Timestamp { get; set; }
    public int SequenceNumber { get; set; }
}
