using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using ShipmentTracking.Entities.DTOs.Auth;
using ShipmentTracking.WebUI.Models;
using System.Security.Claims;
using System.Text;
using System.Net.Http.Headers; // JWT Bileti için eklendi

namespace ShipmentTracking.WebUI.Controllers
{
    // YARDIMCI SINIF: API'den dönen Token ve User verisini tip güvenli (Type-Safe) almak için oluşturduk.
    // Bu sayede dynamic'ten kaynaklanan "null" ve büyük/küçük harf hatalarından kurtuluyoruz!
    public class LoginResponseWrapper
    {
        public string Token { get; set; } = string.Empty;
        public UserResponseViewModel? User { get; set; }
    }
    public class AuthController : Controller
    {
        private readonly HttpClient _httpClient;

        // API'ye istek atmak için HttpClient'ı çağırıyoruz
        public AuthController(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        // 1. GİRİŞ SAYFASINI GÖSTEREN METOT (GET)
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        // 2. GİRİŞ BUTONUNA BASILDIĞINDA ÇALIŞACAK METOT (POST)
        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if ((!ModelState.IsValid))
            {
                return View(model);
            }

            // Müşterinin formda girdiği bilgileri JSON'a çeviriyoruz
            var jsonContent = new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json");

            // API'nin kapısını çalıyoruz (Kendi port numaranı kontrol etmeyi unutma!)
            var response = await _httpClient.PostAsync("https://localhost:7204/api/Auth/login", jsonContent);

            if (response.IsSuccessStatusCode)
            {
                var responseData = await response.Content.ReadAsStringAsync();

                // 1. API'DEN GELEN YAPIYI GÜVENLİ WRAPPER SINIFIMIZ İLE KARŞILIYORUZ
                // Newtonsoft.Json, "User" veya "user" fark etmeksizin otomatik eşleştirme yapacaktır.
                var result = JsonConvert.DeserializeObject<LoginResponseWrapper>(responseData);

                // 2. TOKEN VE USER BİLGİLERİNİN BOŞ OLUP OLMADIĞINI KONTROL EDİYORUZ
                if (result == null || result.User == null || string.IsNullOrEmpty(result.Token))
                {
                    ViewBag.ErrorMessage = "Sunucudan kullanıcı bilgileri veya güvenlik bileti alınamadı!";
                    return View(model);
                }

                string token = result.Token;
                var user = result.User;

                // 3. KULLANICI KİMLİĞİNİ (CLAIMS) OLUŞTURUYORUZ
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier, user!.Id.ToString()),
                    new Claim(ClaimTypes.Name, user.UserName),
                    new Claim(ClaimTypes.GivenName, $"{user.FirstName} {user.LastName}"),
                    new Claim(ClaimTypes.Role, user.Role),
                    // İŞTE SİHİR BURADA! JWT BİLETİNİ DE MVC'NİN ÇEREZİNE (CLAIM OLARAK) SAKLIYORUZ!
                    new Claim("jwt_token", token)
                };

                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

                // 4. TARAYICIYA ÇEREZİ BASIYORUZ (İÇİNDE GİZLİ JWT BİLETİ İLE BİRLİKTE)
                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity));

                TempData["Success"] = $"Hoş geldin, {user.FirstName}!";
                return RedirectToAction("Index", "Shipment");
            }
            else
            {
                // Şifre yanlışsa hata mesajı gösteriyoruz
                ViewBag.ErrorMessage = "Kullanıcı adı veya şifre hatalı!";
                return View(model);
            }
        }

        // 3. ÇIKIŞ YAPMA METODU
        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            // Tarayıcıdaki çerezi siliyoruz (Artık içeride değilsin!)
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            // Çıkış yaptıktan sonra giriş sayfasına yönlendiriyoruz
            return RedirectToAction("Login", "Auth");
        }

        // 4. YENİ PERSONEL KAYIT SAYFASINI GÖSTER (GET)
        [HttpGet]
        [Authorize(Roles = "Admin")] // Sadece Admin personel ekleyebilir
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Register(RegisterViewModel registerViewModel)
        {
            // 1. Kutu kurallara uygun mu? (Boş alan var mı?)
            if (!ModelState.IsValid)
            {
                return View(registerViewModel);
            }

            // --- YENİ EKLENEN KISIM: CEBİMİZDEKİ BİLETİ ÇIKARIYORUZ ---
            // MVC'nin çerezine sakladığımız "jwt_token" isimli bileti buluyoruz
            var token = User.Claims.FirstOrDefault(c => c.Type == "jwt_token")?.Value;

            // Eğer bilet varsa, bunu postacıya (HttpClient) veriyoruz. (Buna Bearer Token denir)
            if (!string.IsNullOrEmpty(token))
            {
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }
            // ---------------------------------------------------------

            // 2. C# Kutusunu (ViewModel) evrensel JSON diline çeviriyoruz
            var jsonContent = new StringContent(JsonConvert.SerializeObject(registerViewModel), Encoding.UTF8, "application/json");

            // 3. API'nin Register kapısını çalıyoruz (Kendi API port numaranı kontrol etmeyi unutma!)
            var response = await _httpClient.PostAsync("https://localhost:7204/api/Auth/register", jsonContent);

            if (response.IsSuccessStatusCode)
            {
                // 4. Kayıt başarılıysa, ekranda yeşil bir mesaj gösterip Kargo Paneline yönlendir
                TempData["Success"] = "Yeni personel başarıyla kaydedildi!";
                return RedirectToAction("Index", "Shipment");
            }
            else
            {
                // 5. API'den hata dönerse (örn: "Bu kullanıcı adı zaten var"), hatayı yakala ve ekranda göster                
                var errorResponse = await response.Content.ReadAsStringAsync();
                ViewBag.ErrorMessage = "Kayıt işlemi sırasında bir hata oluştu!";
                return View(registerViewModel);
            }
        }
    }
}
