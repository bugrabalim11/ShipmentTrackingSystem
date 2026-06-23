using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using ShipmentTracking.Entities.Concrete;
using ShipmentTracking.Entities.DTOs.Shipment;
using ShipmentTracking.Entities.DTOs.ShipmentHistory;
using ShipmentTracking.WebUI.Models;
using System.Text;
using System.Text.Json.Serialization;

namespace ShipmentTracking.WebUI.Controllers
{
    [Authorize]
    [ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)] // GERİ TUŞU HAYALETİNİ ÖLDÜREN KOD
    public class ShipmentController : Controller
    {
        // API ile konuşmamızı sağlayacak sanal tarayıcı nesnemiz
        private readonly HttpClient _httpClient;

        public ShipmentController(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        // 1. Kargo Listeleme Sayfası (Index)
        public async Task<IActionResult> Index()
        {
            var response = await _httpClient.GetAsync("https://localhost:7204/api/Shipments");
            if (response.IsSuccessStatusCode)
            {
                // API'den gelen ham metni (JSON) oku
                var jsonString = await response.Content.ReadAsStringAsync();

                // O metni, C# listesine (List<ShipmentListDto>) dönüştür (Sihirli Kısım)
                var shipments = JsonConvert.DeserializeObject<List<ShipmentListDto>>(jsonString);  // JSON Metni --> C# Nesnesi , Veri okurken

                // Verileri View'a (Arayüze) gönder
                return View(shipments);
            }
            return View(new List<ShipmentListDto>());
        }

        // 2. YENİ KARGO EKLEME SAYFASI (GET - Sadece Boş Formu Gösterir)
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        // 3. YENİ KARGO EKLEME İŞLEMİ (POST - Formu API'ye Gönderir)
        [HttpPost]
        public async Task<IActionResult> Create(ShipmentCreateDto shipmentCreateDto)
        {
            // Kullanıcı zorunlu alanları doldurmadıysa, hatalarla birlikte aynı formu geri gönder
            if (!ModelState.IsValid)
            {
                return View(shipmentCreateDto);
            }

            // 1. DTO'yu API'nin anlayacağı dil olan JSON metnine çeviriyoruz
            var jsonString = JsonConvert.SerializeObject(shipmentCreateDto);  // C# Nesnesi --> JSON Metni , Veri gönderirken

            // 2. Bu metni HTTP üzerinden taşınabilecek bir "Paket" haline getiriyoruz
            var content = new StringContent(jsonString, Encoding.UTF8, "application/json");

            // 3. API'ye POST isteği atıyoruz 
            var response = await _httpClient.PostAsync("https://localhost:7204/api/Shipments", content);

            // Eğer API bize "201 Created" veya "200 OK" gibi başarılı bir yanıt dönerse...
            if (response.IsSuccessStatusCode)
            {
                TempData["Success"] = "Yeni kargo sisteme başarıyla eklendi.";
                return RedirectToAction("Index"); // Kullanıcıyı tekrar listeye yönlendir
            }

            // Başarısız olursa (örneğin API kapalıysa), ekrana hata mesajı bas
            ModelState.AddModelError("", "Kargo eklenirken bir hata oluştu. Lütfen tekrar deneyin.");
            return View(shipmentCreateDto); // Aynı formu geri gönder
        }

        // 4. GÜNCELLEME SAYFASI (GET - İçini dolduracağımız formu API'den getirir)
        [HttpGet]
        public async Task<IActionResult> Update(int id)
        {
            // Önce güncellenecek kargonun mevcut bilgilerini API'den çekiyoruz
            var response = await _httpClient.GetAsync($"https://localhost:7204/api/Shipments/{id}");
            if (response.IsSuccessStatusCode)
            {
                var jsonString = await response.Content.ReadAsStringAsync();

                // Gelen metni DTO'ya çevir (Deserialize)
                var shipment = JsonConvert.DeserializeObject<ShipmentUpdateDto>(jsonString);

                // Formu dolu bir şekilde ekrana gönder
                return View(shipment);
            }
            return RedirectToAction("Index");
        }

        // 5. GÜNCELLEME İŞLEMİ (POST - Değişen verileri API'ye gönderir)
        [HttpPost]
        public async Task<IActionResult> Update(ShipmentUpdateDto shipmentUpdateDto)
        {
            if (!ModelState.IsValid) return View(shipmentUpdateDto); // Hatalarla birlikte aynı formu geri gönder

            // DTO'yu API'ye göndermek için metne çevir (Serialize)
            var jsonString = JsonConvert.SerializeObject(shipmentUpdateDto);
            var content = new StringContent(jsonString, Encoding.UTF8, "application/json");

            // API'ye PUT (Güncelleme) isteği at
            var response = await _httpClient.PutAsync("https://localhost:7204/api/Shipments", content);

            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("Index");
            }

            ModelState.AddModelError("", "Kargo güncellenirken bir hata oluştu. Lütfen tekrar deneyin.");
            return View(shipmentUpdateDto);
        }

        // 6. SİLME İŞLEMİ
        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            // API'ye doğrudan silme komutu gönderiyoruz
            await _httpClient.DeleteAsync($"https://localhost:7204/api/Shipments/{id}");

            TempData["Success"] = "Kargo başarıyla silindi!";
            return RedirectToAction("Index"); // Sildikten sonra sayfayı yenilek
        }

        // 7. KARGO DETAY VE HAREKET GEÇMİŞİ SAYFASI
        [HttpGet]
        public async Task<IActionResult> Detail(int id)
        {
            var viewModel = new ShipmentDetailViewModel();

            // 1. Kargonun ana bilgilerini API'den çekiyoruz
            var shipmentResponse = await _httpClient.GetAsync($"https://localhost:7204/api/Shipments/{id}");
            if (!shipmentResponse.IsSuccessStatusCode)
            {
                return RedirectToAction("Index");  // Kargo bulunamazsa listeye geri dön
            }

            var shipmentJson = await shipmentResponse.Content.ReadAsStringAsync();
            viewModel.Shipment = JsonConvert.DeserializeObject<ShipmentListDto>(shipmentJson)!;

            // 2. Tüm hareket geçmişini API'den çekiyoruz
            var historyResponse = await _httpClient.GetAsync("https://localhost:7204/api/ShipmentHistories");
            if (historyResponse.IsSuccessStatusCode)
            {
                var historyJson = await historyResponse.Content.ReadAsStringAsync();
                var allHistories = JsonConvert.DeserializeObject<List<ShipmentHistoryListDto>>(historyJson);
                if (allHistories != null)
                {
                    // İçinden sadece bu kargoya (ShipmentId) ait olanları filtreleyip, tarihe göre sıralıyoruz
                    viewModel.Histories = allHistories
                        .Where(h => h.ShipmentId == id)
                        .OrderByDescending(h => h.ChangeDate)  // En yeni hareket en üstte gözüksün
                        .ToList();
                }
            }
            // Torbamızı (ViewModel) arayüze gönderiyoruz
            return View(viewModel);
        }

        // 8. YENİ KARGO HAREKETİ EKLEME SAYFASI (GET)
        [HttpGet]
        public IActionResult AddHistory(int id)
        {
            // Ekrana boş bir DTO gönderiyoruz ama HANGİ kargoya ait olduğunu bilsin diye ID'yi içine gizlice koyuyoruz
            var dto = new ShipmentHistoryCreateDto { ShipmentId = id };
            return View(dto);
        }

        // 9. YENİ KARGO HAREKETİ EKLEME İŞLEMİ (POST)
        [HttpPost]
        public async Task<IActionResult> AddHistory(ShipmentHistoryCreateDto dto)
        {
            if (!ModelState.IsValid) return View(dto); // ! uygun değilse

            // DTO'yu API'ye göndermek üzere JSON metnine çevir (Serialize)
            var jsonString = JsonConvert.SerializeObject(dto);
            var content = new StringContent(jsonString, Encoding.UTF8, "application/json");

            // API'deki ShipmentHistories ucuna POST isteği atıyoruz
            var response = await _httpClient.PostAsync("https://localhost:7204/api/ShipmentHistories", content);

            if (response.IsSuccessStatusCode)
            {
                // Başarılı olursa, bizi yeni eklenen hareketleri görmemiz için o kargonun Detay sayfasına geri fırlat!
                TempData["Success"] = "Kargo hareketi başarıyla güncellendi.";
                return RedirectToAction("Detail", new { id = dto.ShipmentId });
            }

            ModelState.AddModelError("", "Hareket eklenirken bir hata oluştu.");
            return View(dto);
        }
    }
}
