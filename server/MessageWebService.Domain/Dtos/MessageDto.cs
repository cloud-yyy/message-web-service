namespace MessageWebService.Domain.Dtos;

public record MessageDto(
    int Id,
    string Text,
    DateTime Timestamp,
    int SequenceNumber
);
