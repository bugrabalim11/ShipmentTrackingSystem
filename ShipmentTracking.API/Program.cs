using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using ShipmentTracking.API.Extensions;
using ShipmentTracking.Business.Abstract;
using ShipmentTracking.Business.Concrete;
using ShipmentTracking.Business.ValidationRules;
using ShipmentTracking.DataAccess.Abstract;
using ShipmentTracking.DataAccess.Concrete.EntityFramework;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// API ve Swagger servisleri
builder.Services.AddControllers();

// --- JWT GÜVENLİK SİSTEMİ KURULUMU ---
// DÜZELTME: "Authentication" yerine "AddAuthentication" olmalıydı!
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true, // Bileti ben mi ürettim kontrol et
            ValidateAudience = true, // Bileti kime ürettim kontrol et
            ValidateLifetime = true, // Biletin süresi geçmiş mi (Örn: 1 saat) kontrol et
            ValidateIssuerSigningKey = true, // Biletin imzası benim gizli anahtarımla mı atılmış kontrol et!

            // appsettings.json'daki ayarları okuyoruz
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!))
        };
    });

// 1. API'ye Gelen İstekleri (JSON verilerini) Otomatik Kontrol Etme Özelliği
builder.Services.AddFluentValidationAutoValidation();

// 2. Sistemi Business katmanındaki kurallarla tanıştırıyoruz
builder.Services.AddValidatorsFromAssemblyContaining<RegisterDtoValidator>();

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

// --- SİHİRLİ SIRALAMA BURADA ---
// ÖNCE kapıdaki görevli bileti kontrol eder (Authentication)
// SONRA içeri giren kişinin rolüne bakar (Authorization)
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();