using MessageWebService.Domain.Abstractions;
using MessageWebService.Domain.Models;
using Npgsql;

namespace MessageWebService.DataAccess.Repositories;

public class MessageRepository(
    string _connectionString) : IMessageRepository
{
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

        await command.ExecuteNonQueryAsync(cancellationToken);
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

        return messages;
    }
}
