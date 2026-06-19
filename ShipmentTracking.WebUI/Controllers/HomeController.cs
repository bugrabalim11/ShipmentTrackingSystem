using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using ShipmentTracking.Entities.Concrete;
using ShipmentTracking.Entities.DTOs.Shipment;
using ShipmentTracking.Entities.DTOs.ShipmentHistory;
using ShipmentTracking.WebUI.Models;

namespace ShipmentTracking.WebUI.Controllers
{
    public class HomeController : Controller
    {
        private readonly HttpClient _httpClient;

        // Sistemin sanal tarayıcıyı (HttpClient) buraya da getirmesini istiyoruz
        public HomeController(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        // Arama işlemi sadece veri "okuduğu" için [HttpGet] kullanıyoruz
        [HttpGet]
        public async Task<IActionResult> Index(string trackingNumber)
        {
            // Eğer sayfa ilk defa açılıyorsa (arama çubuğu boşsa), boş ekranı göster
            if (string.IsNullOrEmpty(trackingNumber))
            {
                return View(null);
            }

            var response = await _httpClient.GetAsync("https://localhost:7204/api/Shipments");

            if (response.IsSuccessStatusCode)
            {
                var jsonString = await response.Content.ReadAsStringAsync();
                var allShipments = JsonConvert.DeserializeObject<List<ShipmentListDto>>(jsonString);

                // API'den gelen listede, müşterinin girdiği Takip Numarasını arıyoruz
                var shipment = allShipments?.FirstOrDefault(s => s.TrackingNumber == trackingNumber);

                if (shipment != null)
                {
                    // Kargo bulundu! Şimdi detaylarını paketlemek için o meşhur ViewModel'i (Sunum Tabağı) hazırlıyoruz
                    var viewModel = new ShipmentDetailViewModel
                    {
                        Shipment = shipment
                    };

                    // Kargonun geçmişini çekiyoruz
                    var historyResponse = await _httpClient.GetAsync($"https://localhost:7204/api/ShipmentHistories");
                    if (historyResponse.IsSuccessStatusCode)
                    {
                        var historyJson = await historyResponse.Content.ReadAsStringAsync();
                        var allHistories = JsonConvert.DeserializeObject<List<ShipmentHistoryListDto>>(historyJson);

                        // Tüm geçmişin içinden sadece bulduğumuz kargonun ID'sine ait olanları süzüyoruz
                        viewModel.Histories = allHistories?
                            .Where(h => h.ShipmentId == shipment.Id)
                            .OrderByDescending(h => h.ChangeDate)
                            .ToList() ?? new List<ShipmentHistoryListDto>();
                    }

                    return View(viewModel); // Dolu tabakla ekrana dön
                }
                else
                {
                    // Kargo bulunamazsa ekrana uyarı mesajı göndermek için ViewBag (Geçici Çanta) kullanıyoruz
                    ViewBag.ErrorMessage = "Bu takip numarasına ait bir kargo bulunamadı. Lütfen numarayı kontrol ediniz.";
                }
            }

            return View(null);
        }
    }
}
