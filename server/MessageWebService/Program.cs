using FluentMigrator.Runner;
using MessageWebService.Application;
using MessageWebService.Application.Services;
using MessageWebService.DataAccess.Migrations;
using MessageWebService.DataAccess.Repositories;
using MessageWebService.Domain.Abstractions;
using MessageWebService.Infrastructure.SignalR;
using MessageWebService.Presentation.Controllers;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddAutoMapper(config =>
    config.AddMaps(typeof(MappingProfile).Assembly));

builder.Services.AddFluentMigratorCore()
    .ConfigureRunner(runnerBuilder => runnerBuilder
        .AddPostgres()
        .WithGlobalConnectionString(builder.Configuration.GetConnectionString("DefaultConnection"))
        .ScanIn(typeof(InitialMigration).Assembly).For.Migrations())
    .AddLogging(loggingBuilder => loggingBuilder.AddFluentMigratorConsole());

builder.Services
    .AddControllers()
    .AddApplicationPart(typeof(MessageController).Assembly);

builder.Services.AddSingleton<IMessageRepository, MessageRepository>();
builder.Services.AddScoped<MessageService>();

var app = builder.Build();

var serviceProvider = app.Services;
using (var scope = serviceProvider.CreateScope())
{
    var runner = scope.ServiceProvider.GetRequiredService<IMigrationRunner>();
    runner.MigrateUp();
}

app.MapControllers();

app.MapHub<MessageHub>("/message-hub");

app.Run();
