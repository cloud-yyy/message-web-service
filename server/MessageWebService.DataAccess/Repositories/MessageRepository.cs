using MessageWebService.Domain.Abstractions;
using MessageWebService.Domain.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Npgsql;

namespace MessageWebService.DataAccess.Repositories;

public class MessageRepository(
    IConfiguration configuration,
    ILogger<MessageRepository> logger) : IMessageRepository
{
    private readonly string _connectionString
        = configuration.GetConnectionString("DefaultConnection")!;

    private readonly ILogger<MessageRepository> logger = logger;

    public async Task SaveMessageAsync(
        Message message, CancellationToken cancellationToken = default)
    {
        using var connection = new NpgsqlConnection(_connectionString);
        await connection.OpenAsync(cancellationToken);

        var sql =
        """
        INSERT INTO messages (text, timestamp, sequence_number)
        VALUES (@text, @timestamp, @sequenceNumber)
        """;

        using var command = new NpgsqlCommand(sql, connection);

        command.Parameters.AddWithValue("text", message.Text);
        command.Parameters.AddWithValue("timestamp", message.Timestamp);
        command.Parameters.AddWithValue("sequenceNumber", message.SequenceNumber);

        try
        {
            await command.ExecuteNonQueryAsync(cancellationToken);

            logger.LogInformation(
                "Message {sequenceNumber} saved with text '{text}' at {timestamp}",
                message.SequenceNumber, message.Text, message.Timestamp);
        }
        catch (NpgsqlException ex)
        {
            logger.LogError(ex,
                "Error saving message {sequenceNumber} with text '{text}' at {timestamp}",
                message.SequenceNumber, message.Text, message.Timestamp);
            throw;
        }
    }

    public async Task<IEnumerable<Message>> GetMessagesAsync(
        DateTime start, DateTime end, CancellationToken cancellationToken = default)
    {
        var messages = new List<Message>();

        using var connection = new NpgsqlConnection(_connectionString);
        await connection.OpenAsync(cancellationToken);

        var sql =
        """
        SELECT text, timestamp, sequence_number
        FROM messages
        WHERE timestamp BETWEEN @start AND @end
        """;

        using var command = new NpgsqlCommand(sql, connection);

        command.Parameters.AddWithValue("start", start);
        command.Parameters.AddWithValue("end", end);

        try
        {
            using var reader = await command.ExecuteReaderAsync(cancellationToken);

            while (await reader.ReadAsync(cancellationToken))
            {
                messages.Add(new Message
                {
                    Text = reader.GetString(0),
                    Timestamp = reader.GetDateTime(1),
                    SequenceNumber = reader.GetInt32(2)
                });
            }

            logger.LogInformation(
                "Retrieved {count} messages from {start} to {end}",
                messages.Count, start, end);

            return messages;
        }
        catch (NpgsqlException ex)
        {
            logger.LogError(ex,
                "Error getting messages from {start} to {end}",
                start, end);
            throw;
        }
    }
}

