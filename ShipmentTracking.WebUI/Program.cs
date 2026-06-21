using Microsoft.AspNetCore.Authentication.Cookies;

var builder = WebApplication.CreateBuilder(args);

// Sisteme MVC yeteneklerini ekler
builder.Services.AddControllersWithViews();

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
