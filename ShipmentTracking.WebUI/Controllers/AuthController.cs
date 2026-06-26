using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using ShipmentTracking.Entities.DTOs.Auth;
using ShipmentTracking.Entities.DTOs.Shipment;
using ShipmentTracking.WebUI.Models;
using System.Net.Http.Headers; // JWT Bileti için eklendi
using System.Security.Claims;
using System.Text;

namespace ShipmentTracking.WebUI.Controllers
{
    // =========================================================================
    // [ÖĞRETMEN NOTU - Neden LoginResponseWrapper Yazdık?]
    // Eskiden API (Kasa) şifre doğruysa bize sadece Müşterinin adını yolluyordu.
    // Artık API bize bir "Kutu" yolluyor. Bu kutunun içinde hem kullanıcının bilgileri (User)
    // hem de V.I.P Giriş Bileti (Token) var.
    // C#'ın kafası karışmasın, "Gelen kutudan ne çıkacak?" diye çökmesin diye 
    // bu kalıbı (Wrapper) yazdık. "Gelen veriyi bu kalıba dök" diyoruz.
    // =========================================================================
    public class LoginResponseWrapper
    {
        public string Token { get; set; } = string.Empty;
        public UserResponseViewModel? User { get; set; }
    }

    // =========================================================================
    // [ÖĞRETMEN NOTU - GERİ TUŞU HAYALETİNİ ÖLDÜREN KOD]
    // Çıkış (Logout) yaptıktan sonra tarayıcıdaki "Geri" okuna basınca, 
    // tarayıcı eski sayfaların fotoğrafını çektiği (Cache) için adamı içerideymiş gibi gösteriyordu.
    // Bu satır tarayıcıya şunu der: "Sakın sayfaların fotoğrafını çekme! 
    // Her sayfa değişiminde canlı olarak sunucuya sor!"
    // =========================================================================
    [ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)]
    public class AuthController : Controller
    {
        private readonly HttpClient _httpClient;

        // API'ye istek atmak için HttpClient'ı çağırıyoruz
        public AuthController(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        private void AttachToken()
        {
            // Giriş yaparken cüzdana (Cookie) sakladığımız bileti buluyoruz
            var token = User.Claims.FirstOrDefault(c => c.Type == "jwt_token")?.Value;

            if (!string.IsNullOrEmpty(token))
            {
                // Sanal postacımızın yaka kartına "Bearer [Bilet]" şeklinde iğneliyoruz
                _httpClient.DefaultRequestHeaders.Authorization =
                    new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            }
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

                // 1. API'DEN GELEN KOLİYİ AÇIYORUZ
                // Koli geldi, kalıbımıza (LoginResponseWrapper) döktük.
                var result = JsonConvert.DeserializeObject<LoginResponseWrapper>(responseData);

                // 2. TOKEN VE USER BİLGİLERİNİN BOŞ OLUP OLMADIĞINI KONTROL EDİYORUZ
                if (result == null || result.User == null || string.IsNullOrEmpty(result.Token))
                {
                    ViewBag.ErrorMessage = "Sunucudan kullanıcı bilgileri veya güvenlik bileti alınamadı!";
                    return View(model);
                }

                // Kolinin içindeki V.I.P Bileti ve Müşteriyi ayrı ayrı elimize aldık.
                string token = result.Token;
                var user = result.User;

                // 3. KULLANICI KİMLİĞİNİ (CLAIMS) OLUŞTURUYORUZ
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier, user!.Id.ToString()),
                    new Claim(ClaimTypes.Name, user.UserName),
                    new Claim(ClaimTypes.GivenName, $"{user.FirstName} {user.LastName}"),
                    new Claim(ClaimTypes.Role, user.Role),
                    
                    // =========================================================================
                    // [ÖĞRETMEN NOTU - EN ÖNEMLİ SİHİR BURADA!]
                    // V.I.P Bileti (Token'ı) adamın elinde taşıtmak yerine, 
                    // MVC'nin oluşturduğu "Çerezin" (Cüzdanın) en gizli köşesine koyuyoruz.
                    // Adam sitede dolaşırken bu bilet hep arka planda onunla gezecek.
                    // =========================================================================
                    new Claim("jwt_token", token)
                };

                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

                // 4. TARAYICIYA ÇEREZİ BASIYORUZ (Adam artık içeride!)
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
            // Tarayıcıdaki çerezi (ve içindeki saklı JWT Biletini) çöpe atıyoruz.
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
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

            // =========================================================================
            // [ÖĞRETMEN NOTU - POSTACIYA BİLETİ VERMEK]
            // MVC tarafında "Kaydet" butonuna basıldı. MVC, API'nin (Kasanın) kapısına gidecek.
            // Ama API'nin kapısında artık yeni bir kilit var, BİLET (Token) istiyor!
            // Hemen adamın cüzdanına (User.Claims) bakıyoruz. "jwt_token" isimli bileti buluyoruz.
            // =========================================================================
            var token = User.Claims.FirstOrDefault(c => c.Type == "jwt_token")?.Value;

            // Bileti bulduysak, Sanal Postacımızın(HttpClient) yaka kartına(Authorization Header)
            // bu bileti iliştiriyoruz. API kapıdaki bu yaka kartını görünce içeri alacak!
            if (!string.IsNullOrEmpty(token))
            {
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }
            // ---------------------------------------------------------

            // 2. C# Kutusunu (ViewModel) evrensel JSON diline çeviriyoruz
            var jsonContent = new StringContent(JsonConvert.SerializeObject(registerViewModel), Encoding.UTF8, "application/json");

            // Postacı yola çıkıyor...
            var response = await _httpClient.PostAsync("https://localhost:7204/api/Auth/register", jsonContent);

            if (response.IsSuccessStatusCode)
            {
                TempData["Success"] = "Yeni personel başarıyla kaydedildi!";
                return RedirectToAction("Index", "Shipment");
            }
            else
            {
                ViewBag.ErrorMessage = "Kayıt işlemi sırasında bir hata oluştu!";
                return View(registerViewModel);
            }
        }

        // =========================================================================
        // [ÖĞRETMEN NOTU - PERSONEL YÖNETİMİ]
        // =========================================================================

        // BÜTÜN PERSONELLERİ LİSTELEYEN SAYFA (GET)
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> PersonnelList()
        {
            // 1. BİLETİ ÇIKAR: MVC cüzdanından (Cookie) JWT biletini alıyoruz.
            var token = User.Claims.FirstOrDefault(c => c.Type == "jwt_token")?.Value;
            if (!string.IsNullOrEmpty(token))
            {
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }

            // 2. KASAYA GİT: API'nin GetAllPersonnel kapısını çalıyoruz.
            var response = await _httpClient.GetAsync("https://localhost:7204/api/Auth/GetAllPersonnel");

            if (response.IsSuccessStatusCode)
            {
                var responseData = await response.Content.ReadAsStringAsync();

                // Gelen JSON verisini C# listesine çevirip Vitrin'e (View) yolluyoruz.
                var personnelList = JsonConvert.DeserializeObject<List<UserResponseViewModel>>(responseData);
                return View(personnelList);
            }

            ViewBag.ErrorMessage = "Personel listesi yüklenirken bir hata oluştu!";
            return View(new List<UserResponseViewModel>());
        }

        // PERSONEL SİLME İŞLEMİ (POST)
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeletePersonnel(int id)
        {
            // 1. BİLETİ ÇIKAR
            var token = User.Claims.FirstOrDefault(c => c.Type == "jwt_token")?.Value;
            if (!string.IsNullOrEmpty(token))
            {
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }

            // 2. KASAYA GİT: API'nin silme kapısına personelin ID'sini (Örn: /5) gönderiyoruz.
            var response = await _httpClient.DeleteAsync($"https://localhost:7204/api/Auth/DeletePersonnel/{id}");

            if (response.IsSuccessStatusCode)
            {
                TempData["Success"] = "Personel başarıyla silindi!";
            }
            // SADECE 400 (BadRequest) dönerse bizim API'de yazdığımız mesajdır ("Admin silinemez" gibi)
            else if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
            {
                TempData["Error"] = await response.Content.ReadAsStringAsync();
            }
            // EĞER 500 GİBİ BİR SUNUCU ÇÖKMESİ VARSA (İlişkili veriler silinemez vb.)
            else
            {
                TempData["Error"] = "İşlem başarısız! Bu personele ait kayıtlı kargolar olabilir.";
            }

            // İşlem bitince sayfayı yenilemek için aynı sayfaya yönlendiriyoruz.
            return RedirectToAction("PersonnelList");
        }

        // PERSONELİN İŞLEMLERİNİ (KARGO GEÇMİŞİ) LİSTELEME
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> PersonnelShipments(int id, string personnelName)
        {
            AttachToken();
            ViewBag.PersonnelName = personnelName;

            var response = await _httpClient.GetAsync($"https://localhost:7204/api/Shipments/GetByPersonnel/{id}");

            if (response.IsSuccessStatusCode)
            {
                var data = await response.Content.ReadAsStringAsync();
                var list = JsonConvert.DeserializeObject<List<ShipmentListDto>>(data);
                return View(list);
            }

            return View(new List<ShipmentListDto>());
        }
    }
}
