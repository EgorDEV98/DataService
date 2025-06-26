using System.Text.Json.Serialization;
using DataService.Application;
using DataService.Application.Extensions;
using DataService.Data;
using DataService.Data.Extensions;
using DataService.Integration.Tinkoff;
using DataService.WebApi.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
        options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
    });
builder.Services.AddPostgres(builder.Configuration);
builder.Services.AddTinkoff(builder.Configuration);
builder.Services.AddApplication(builder.Configuration);
builder.Services.AddMappers();

var app = builder.Build();
await app.Services.ApplyMigrationAsync();
await app.Services.PreloadSharesAsync();
await app.Services.PreloadSchedulersAsync();

app.MapControllers();
app.UseSwagger();
app.UseSwaggerUI();
app.UseHttpsRedirection();
app.Run();