using DataService.Data;
using DataService.Data.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddPostgres(builder.Configuration);

var app = builder.Build();
await app.Services.ApplyMigrationAsync();

app.UseSwagger();
app.UseSwaggerUI();
app.UseHttpsRedirection();
app.Run();