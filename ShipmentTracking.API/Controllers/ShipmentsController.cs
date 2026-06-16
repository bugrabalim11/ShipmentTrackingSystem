using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ShipmentTracking.Business.Abstract;
using ShipmentTracking.Entities.Concrete;

namespace ShipmentTracking.API.Controllers
{
    // Dış dünyadan bu sınıfa nasıl ulaşılacağının adresi: "localhost:5000/api/shipments"
    [Route("api/[controller]")]
    // Bu sınıfın bir API denetleyicisi olduğunu belirtir. Otomatik doğrulama gibi özellikleri açar.
    [ApiController]
    public class ShipmentsController : ControllerBase
    {
        private readonly IShipmentService _shipmentService;

        // Dependency Injection: Resepsiyoniste, çalışacağı şube müdürünü (Servisi) veriyoruz.
        public ShipmentsController(IShipmentService shipmentService)
        {
            _shipmentService = shipmentService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var shipments = await _shipmentService.GetListAsync();
            return Ok(shipments); // Ok() -> HTTP 200 Başarılı koduyla veriyi döner.
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var shipment = await _shipmentService.GetByIdAsync(id);
            if (shipment == null)
            {
                return NotFound();  // NotFound() -> HTTP 404 Bulunamadı kodu döner.
            }
            return Ok(shipment);
        }

        [HttpPost]
        // Kargoyu başarıyla oluşturdum(201).Eğer bu kargonun detaylarına bakmak istersen GetById metoduna git,
        // içine de az önce ürettiğim ID'yi ver. Al bu da oluşturduğum kargonun son hali (shipment).

        // Kullanıcı URL'ye bir şey yazmayacak, sen verileri gelen isteğin görünmeyen gövdesinden
        // (JSON) al ve Shipment sınıfına çevir.
        public async Task<IActionResult> Add([FromBody] Shipment shipment)
        {
            await _shipmentService.AddAsync(shipment);
            // Kargo eklendikten sonra "Başarıyla oluşturuldu" (HTTP 201) mesajı dönüyoruz.
            return CreatedAtAction(nameof(GetById), new { id = shipment.Id }, shipment);
        }

        [HttpPut]
        public async Task<IActionResult> Update([FromBody] Shipment shipment)
        {
            await _shipmentService.UpdateAsync(shipment);
            return NoContent(); // Güncelleme başarılı ama geriye dönecek yeni bir veri yok (HTTP 204).
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var shipment = await _shipmentService.GetByIdAsync(id);
            if (shipment == null)
            {
                return NotFound();
            }

            await _shipmentService.DeleteAsync(shipment);
            return NoContent();
        }
    }
}
