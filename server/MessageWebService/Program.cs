using FluentMigrator.Runner;
using MessageWebService.Application;
using MessageWebService.Application.Services;
using MessageWebService.DataAccess.Migrations;
using MessageWebService.DataAccess.Repositories;
using MessageWebService.Domain.Abstractions;
using MessageWebService.Infrastructure.SignalR;
using MessageWebService.Presentation.Controllers;
using Microsoft.OpenApi.Models;

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

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyHeader();
        policy.AllowAnyMethod();
        policy.AllowCredentials();
        policy.WithOrigins("http://localhost:8081"); 
    });
});

builder.Services.AddSignalR();

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Message API", Version = "v1" });
});

builder.Services.AddSingleton<IMessageRepository, MessageRepository>();
builder.Services.AddSingleton<IMessageSender, MesageSender>();
builder.Services.AddScoped<IMessageService, MessageService>();

var app = builder.Build();

var serviceProvider = app.Services;
using (var scope = serviceProvider.CreateScope())
{
    var runner = scope.ServiceProvider.GetRequiredService<IMigrationRunner>();
    runner.MigrateUp();
}

app.UseCors();

app.UseSwagger();
app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Message API V1"));

app.MapControllers();

app.MapHub<MessageHub>("/message-hub");

app.Run();
