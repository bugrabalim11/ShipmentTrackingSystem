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

        // Artık okyanusu çekmiyoruz, sadece istediğimiz kargoyu API'den noktasal olarak istiyoruz.
        [HttpGet]
        public async Task<IActionResult> Index(string trackingNumber)
        {
            if (string.IsNullOrEmpty(trackingNumber)) return View(null);

            // 1. API'ye Diyoruz ki: "Bana SADECE şu numaralı kargoyu ver!"
            var response = await _httpClient.GetAsync($"https://localhost:7204/api/Shipments/GetByTrackingNumber/{trackingNumber}");

            if (response.IsSuccessStatusCode)
            {
                var jsonString = await response.Content.ReadAsStringAsync();
                var shipmnet = JsonConvert.DeserializeObject<ShipmentListDto>(jsonString);

                if (shipmnet != null)
                {
                    var viewModel = new ShipmentDetailViewModel
                    {
                        Shipment = shipmnet
                    };

                    // 2. API'ye Diyoruz ki: "Bana SADECE bu kargonun geçmişini ver!"
                    var historyResponse = await _httpClient.GetAsync($"https://localhost:7204/api/ShipmentHistories/GetByShipmentId/{shipmnet.Id}");

                    if (historyResponse.IsSuccessStatusCode)
                    {
                        var historyJson = await historyResponse.Content.ReadAsStringAsync();
                        var shipmentHistories = JsonConvert.DeserializeObject<List<ShipmentHistoryListDto>>(historyJson);

                        viewModel.Histories = shipmentHistories?
                            .OrderByDescending(h => h.ChangeDate)
                            .ToList() ?? new List<ShipmentHistoryListDto>();
                    }

                    return View(viewModel);  // Dolu tabakla ekrana dön
                }
            }

            // Kargo bulunamazsa veya 404/401 dönerse
            ViewBag.ErrorMessage = "Bu takip numarasına ait bir kargo bulunamadı. Lütfen numarayı kontrol ediniz.";
            return View(null);
        }
    }
}
