using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using ShipmentTracking.Entities.Concrete;
using ShipmentTracking.Entities.DTOs.Shipment;
using ShipmentTracking.Entities.DTOs.ShipmentHistory;
using ShipmentTracking.WebUI.Models;
using System.Text;
using System.Text.Json.Serialization;
using System.Net.Http.Headers;

namespace ShipmentTracking.WebUI.Controllers
{
    [Authorize]
    [ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)]
    public class ShipmentController : Controller
    {
        private readonly HttpClient _httpClient;

        public ShipmentController(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        // =========================================================================
        // 🎯 SİHİRLİ METOT: API'ye gitmeden önce Cüzdandaki Bileti Gösterir
        // =========================================================================
        private void AttachToken()
        {
            var token = User.Claims.FirstOrDefault(c => c.Type == "jwt_token")?.Value;

            if (!string.IsNullOrEmpty(token))
            {
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }
        }

        // 1. Kargo Listeleme Sayfası (Index)
        public async Task<IActionResult> Index()
        {
            AttachToken(); // 🛑 POSTACIYA BİLETİNİ TAKTIK!

            var response = await _httpClient.GetAsync("https://localhost:7204/api/Shipments");
            if (response.IsSuccessStatusCode)
            {
                var jsonString = await response.Content.ReadAsStringAsync();
                var shipments = JsonConvert.DeserializeObject<List<ShipmentListDto>>(jsonString);
                return View(shipments);
            }
            return View(new List<ShipmentListDto>());
        }

        // 2. YENİ KARGO EKLEME SAYFASI (GET)
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        // 3. YENİ KARGO EKLEME İŞLEMİ (POST)
        [HttpPost]
        public async Task<IActionResult> Create(ShipmentCreateDto shipmentCreateDto)
        {
            AttachToken(); // 🛑 BİLETİ TAKTIK!

            if (!ModelState.IsValid)
            {
                return View(shipmentCreateDto);
            }

            var jsonString = JsonConvert.SerializeObject(shipmentCreateDto);
            var content = new StringContent(jsonString, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync("https://localhost:7204/api/Shipments", content);

            if (response.IsSuccessStatusCode)
            {
                TempData["Success"] = "Yeni kargo sisteme başarıyla eklendi.";
                return RedirectToAction("Index");
            }

            ModelState.AddModelError("", "Kargo eklenirken bir hata oluştu. Lütfen tekrar deneyin.");
            return View(shipmentCreateDto);
        }

        // 4. GÜNCELLEME SAYFASI (GET)
        [HttpGet]
        public async Task<IActionResult> Update(int id)
        {
            AttachToken(); // 🛑 BİLETİ TAKTIK!

            var response = await _httpClient.GetAsync($"https://localhost:7204/api/Shipments/{id}");
            if (response.IsSuccessStatusCode)
            {
                var jsonString = await response.Content.ReadAsStringAsync();
                var shipment = JsonConvert.DeserializeObject<ShipmentUpdateDto>(jsonString);
                return View(shipment);
            }
            return RedirectToAction("Index");
        }

        // 5. GÜNCELLEME İŞLEMİ (POST)
        [HttpPost]
        public async Task<IActionResult> Update(ShipmentUpdateDto shipmentUpdateDto)
        {
            AttachToken(); // 🛑 BİLETİ TAKTIK!

            if (!ModelState.IsValid) return View(shipmentUpdateDto);

            var jsonString = JsonConvert.SerializeObject(shipmentUpdateDto);
            var content = new StringContent(jsonString, Encoding.UTF8, "application/json");

            var response = await _httpClient.PutAsync("https://localhost:7204/api/Shipments", content);

            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("Index");
            }

            ModelState.AddModelError("", "Kargo güncellenirken bir hata oluştu. Lütfen tekrar deneyin.");
            return View(shipmentUpdateDto);
        }

        // 6. SİLME İŞLEMİ
        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            AttachToken(); // 🛑 BİLETİ TAKTIK!

            var response = await _httpClient.DeleteAsync($"https://localhost:7204/api/Shipments/{id}");

            if (response.IsSuccessStatusCode)
            {
                TempData["Success"] = "Kargo başarıyla silindi!";
            }
            else
            {
                TempData["Error"] = "Silerken bir hata oluştu. Yetkiniz olmayabilir.";
            }

            return RedirectToAction("Index");
        }

        // 7. KARGO DETAY VE HAREKET GEÇMİŞİ SAYFASI
        [HttpGet]
        public async Task<IActionResult> Detail(int id)
        {
            AttachToken(); // 🛑 BİLETİ TAKTIK!

            var viewModel = new ShipmentDetailViewModel();

            var shipmentResponse = await _httpClient.GetAsync($"https://localhost:7204/api/Shipments/{id}");
            if (!shipmentResponse.IsSuccessStatusCode)
            {
                return RedirectToAction("Index");
            }

            var shipmentJson = await shipmentResponse.Content.ReadAsStringAsync();
            viewModel.Shipment = JsonConvert.DeserializeObject<ShipmentListDto>(shipmentJson)!;

            var historyResponse = await _httpClient.GetAsync("https://localhost:7204/api/ShipmentHistories");
            if (historyResponse.IsSuccessStatusCode)
            {
                var historyJson = await historyResponse.Content.ReadAsStringAsync();
                var allHistories = JsonConvert.DeserializeObject<List<ShipmentHistoryListDto>>(historyJson);
                if (allHistories != null)
                {
                    viewModel.Histories = allHistories
                        .Where(h => h.ShipmentId == id)
                        .OrderByDescending(h => h.ChangeDate)
                        .ToList();
                }
            }
            return View(viewModel);
        }

        // 8. YENİ KARGO HAREKETİ EKLEME SAYFASI (GET)
        [HttpGet]
        public IActionResult AddHistory(int id)
        {
            var dto = new ShipmentHistoryCreateDto { ShipmentId = id };
            return View(dto);
        }

        // 9. YENİ KARGO HAREKETİ EKLEME İŞLEMİ (POST)
        [HttpPost]
        public async Task<IActionResult> AddHistory(ShipmentHistoryCreateDto dto)
        {
            AttachToken(); // 🛑 BİLETİ TAKTIK!

            if (!ModelState.IsValid) return View(dto);

            var jsonString = JsonConvert.SerializeObject(dto);
            var content = new StringContent(jsonString, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync("https://localhost:7204/api/ShipmentHistories", content);

            if (response.IsSuccessStatusCode)
            {
                TempData["Success"] = "Kargo hareketi başarıyla güncellendi.";
                return RedirectToAction("Detail", new { id = dto.ShipmentId });
            }

            ModelState.AddModelError("", "Hareket eklenirken bir hata oluştu.");
            return View(dto);
        }
    }
}