using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.Cookies;
using ShipmentTracking.WebUI.ValidationRules;

var builder = WebApplication.CreateBuilder(args);

// .NET'in varsayılan İngilizce 'Zorunlu Alan' (Implicit Required) hatalarını susturuyoruz.
// Kontrolü tamamen kendi yazdığımız FluentValidation sınıflarına bırakıyoruz!
builder.Services.AddControllersWithViews(options =>
{
    options.SuppressImplicitRequiredAttributeForNonNullableReferenceTypes = true;
});

// 2. FluentValidation'ın Otomatik Kontrol (Server-side) ve Tarayıcı (Client-side) yeteneklerini ekliyoruz
builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddFluentValidationClientsideAdapters();

// 3. Projedeki Güvenlik Görevlilerini (Validator'ları) bulup sisteme tanıtıyoruz
builder.Services.AddValidatorsFromAssemblyContaining<RegisterViewModelValidator>();

// Çerez (Cookie) makinesini sisteme tanıtıyoruz
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.Cookie.Name = "ShipmentTrackingCookie"; // Çerezin adı
        options.LoginPath = "/Auth/Login";              // Giriş yapmamış adamı buraya fırlat
        options.AccessDeniedPath = "/Auth/Login";       // Yetkisi olmayan adamı da buraya fırlat
        options.ExpireTimeSpan = TimeSpan.FromHours(2); // 2 saat sonra otomatik çıkış yap
    });

// ---> EKLENECEK SİHİRLİ SATIR <---
// Sisteme HttpClient (Sanal Tarayıcı) yeteneğini öğretir
builder.Services.AddHttpClient();

var app = builder.Build();


// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthentication(); // 1. Sen kimsin? (Kimlik kontrolü)
app.UseAuthorization();  // 2. Girmeye yetkin var mı? (Yetki kontrolü)

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();


app.Run();
