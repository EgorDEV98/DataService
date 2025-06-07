using DataService.Application;
using DataService.Application.Interfaces;
using DataService.Data;
using DataService.Data.Extensions;
using DataService.Integration.Tinkoff;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDataLayer(builder.Configuration);
builder.Services.AddApplicationLayer(builder.Configuration);
builder.Services.AddTinkoffIntegration(builder.Configuration);

var app = builder.Build();
await app.Services.ApplyMigrationAsync();

// Первичная синхронизация акций
await using var scope = app.Services.CreateAsyncScope();
var service = scope.ServiceProvider.GetRequiredService<ISyncShareService>();
await service.SyncSharesAsync();

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();
app.Run();
