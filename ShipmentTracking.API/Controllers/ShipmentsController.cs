using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ShipmentTracking.Business.Abstract;
using ShipmentTracking.Entities.Concrete;
using ShipmentTracking.Entities.DTOs.Shipment;
using System.Security.Claims;

namespace ShipmentTracking.API.Controllers
{
    // Dış dünyadan bu sınıfa nasıl ulaşılacağının adresi: "localhost:5000/api/shipments"
    [Route("api/[controller]")]
    // Bu sınıfın bir API denetleyicisi olduğunu belirtir. Otomatik doğrulama gibi özellikleri açar.
    [ApiController]
    [Authorize]
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
            var result = await _shipmentService.GetListAsync();
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _shipmentService.GetByIdAsync(id);
            if (result == null)
            {
                return NotFound();  // Bulunamazsa 404 dönüyoruz
            }
            return Ok(result);
        }

        // 🎯 MÜHÜRLEME HAZIRLIĞI BURADA BAŞLIYOR!
        [HttpPost]
        public async Task<IActionResult> Add(ShipmentCreateDto shipmentCreateDto)
        {
            // 1. Giren personelin cebindeki token'dan ID (Kimlik) bilgisini okuyoruz
            var userIdClaim = User.Claims.FirstOrDefault(c =>
                c.Type == ClaimTypes.NameIdentifier ||
                c.Type == System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.Sub ||
                c.Type == "Sub");

            // 2. Eğer biletin içinden bu ID'yi başarıyla okuduysak...
            if (userIdClaim != null && int.TryParse(userIdClaim.Value, out int currentUserId))
            {
                // ...Kargoya "Bu kargoyu şu ID'li personel ekledi" mührünü basıyoruz!
                shipmentCreateDto.AppUserId = currentUserId;
            }

            // 3. Mühürlü kargoyu servise (veritabanına) gönderiyoruz
            await _shipmentService.AddAsync(shipmentCreateDto);
            return StatusCode(201);
        }

        [HttpPut]
        public async Task<IActionResult> Update(ShipmentUpdateDto shipmentUpdateDto)
        {
            // İleride buraya: "Admin değilse ve başkasının kargosunu güncelliyorsa yasakla (403)" kodunu ekleyeceğiz.
            await _shipmentService.UpdateAsync(shipmentUpdateDto);
            return Ok();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var shipment = await _shipmentService.GetByIdAsync(id);
            if (shipment == null)
            {
                return NotFound();
            }

            // İleride buraya: "Sadece kendi kargonu silebilirsin" güvenlik kodunu ekleyeceğiz.
            await _shipmentService.DeleteAsync(id);
            return Ok();
        }
    }
}
