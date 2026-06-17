using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using ShipmentTracking.Entities.DTOs.Shipment;
using System.Text.Json.Serialization;

namespace ShipmentTracking.WebUI.Controllers
{
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
                var shipments = JsonConvert.DeserializeObject<List<ShipmentListDto>>(jsonString);

                // Verileri View'a (Arayüze) gönder
                return View(shipments);
            }
            return View(new List<ShipmentListDto>());
        }
    }
}
