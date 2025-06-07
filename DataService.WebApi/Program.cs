using DataService.Data;
using DataService.Data.Extensions;
using DataService.Integration.Tinkoff;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDataLayer(builder.Configuration);
builder.Services.AddTinkoffIntegration(builder.Configuration);

var app = builder.Build();
await app.Services.ApplyMigrationAsync();

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();
app.Run();
