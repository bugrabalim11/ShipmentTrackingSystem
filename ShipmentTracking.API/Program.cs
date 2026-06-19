using Microsoft.EntityFrameworkCore;
using ShipmentTracking.API.Extensions;
using ShipmentTracking.Business.Abstract;
using ShipmentTracking.Business.Concrete;
using ShipmentTracking.DataAccess.Abstract;
using ShipmentTracking.DataAccess.Concrete.EntityFramework;

var builder = WebApplication.CreateBuilder(args);

// API ve Swagger servisleri
builder.Services.AddControllers();

// AutoMapper'ı sisteme dahil ediyoruz ve sözlüğümüzün yerini gösteriyoruz
builder.Services.AddAutoMapper(cfg => { }, typeof(ShipmentTracking.Business.Profiles.MappingProfile).Assembly);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Veritabanı bağlantısı
builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"));
});

// Senin Business servislerin
builder.Services.AddBusinessServices();

var app = builder.Build();

// Swagger'ı arayüze ekleme kısmı
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();