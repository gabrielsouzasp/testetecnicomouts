using Microsoft.EntityFrameworkCore;
using Mouts.SalesRecords.Application.Core;
using Mouts.SalesRecords.Infra.Database;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddOpenApiDocument(config =>
{
    config.Title = "Motus.SalesRecords.Api";
});
builder.Services.AddSalesRecordsInjection();

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<SalesContext>(options =>
    options.UseNpgsql(connectionString));


var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseHttpsRedirection();

if(app.Environment.IsDevelopment())
{
    app.UseOpenApi();
    app.UseSwaggerUi();
}

app.UseAuthorization();

app.MapControllers();

app.Run();
