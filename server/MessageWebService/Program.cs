using FluentMigrator.Runner;
using MessageWebService.DataAccess.Migrations;
using MessageWebService.DataAccess.Repositories;
using MessageWebService.Domain.Abstractions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddFluentMigratorCore()
    .ConfigureRunner(rb => rb
        .AddPostgres()
        .WithGlobalConnectionString(builder.Configuration.GetConnectionString("DefaultConnection"))
        .ScanIn(typeof(InitialMigration).Assembly).For.Migrations())
    .AddLogging(lb => lb.AddFluentMigratorConsole());

builder.Services.AddSingleton<IMessageRepository, MessageRepository>();

var app = builder.Build();

var serviceProvider = app.Services;
using (var scope = serviceProvider.CreateScope())
{
    var runner = scope.ServiceProvider.GetRequiredService<IMigrationRunner>();
    runner.MigrateUp();
}

app.MapGet("/", () => "Hello World!");

app.Run();
