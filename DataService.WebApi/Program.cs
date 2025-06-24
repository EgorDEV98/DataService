using DataService.Application;
using DataService.Application.Extensions;
using DataService.Data;
using DataService.Data.Extensions;
using DataService.Integration.Tinkoff;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddPostgres(builder.Configuration);
builder.Services.AddTinkoff(builder.Configuration);
builder.Services.AddApplication(builder.Configuration);

var app = builder.Build();
await app.Services.ApplyMigrationAsync();
await app.Services.PreloadSharesAsync();

app.UseSwagger();
app.UseSwaggerUI();
app.UseHttpsRedirection();
app.Run();