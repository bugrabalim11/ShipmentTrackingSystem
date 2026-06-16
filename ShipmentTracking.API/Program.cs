using ShipmentTracking.API.Extensions;
using ShipmentTracking.Business.Abstract;
using ShipmentTracking.Business.Concrete;
using ShipmentTracking.DataAccess.Abstract;
using ShipmentTracking.DataAccess.Concrete.EntityFramework;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

// Program.cs içinde sadece bunu çağırman yeterli:
builder.Services.AddBusinessServices();

builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
