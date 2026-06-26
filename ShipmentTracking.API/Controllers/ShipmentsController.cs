using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ShipmentTracking.Business.Abstract;
using ShipmentTracking.Entities.Concrete;
using ShipmentTracking.Entities.DTOs.Shipment;
using System.Security.Claims;

namespace ShipmentTracking.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize] // Sınıf bazında güvenlik
    public class ShipmentsController : ControllerBase
    {
        private readonly IShipmentService _shipmentService;

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
                return NotFound();
            }
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> Add(ShipmentCreateDto shipmentCreateDto)
        {
            var userIdClaim = User.Claims.FirstOrDefault(c =>
                c.Type == ClaimTypes.NameIdentifier ||
                c.Type == System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.Sub ||
                c.Type == "sub");

            if (userIdClaim != null && int.TryParse(userIdClaim.Value, out int currentUserId))
            {
                shipmentCreateDto.AppUserId = currentUserId;
            }

            await _shipmentService.AddAsync(shipmentCreateDto);
            return StatusCode(201);
        }

        [HttpPut]
        public async Task<IActionResult> Update(ShipmentUpdateDto shipmentUpdateDto)
        {
            // 1. Orijinal kargoyu bul
            var existingShipment = await _shipmentService.GetByIdAsync(shipmentUpdateDto.Id);
            if (existingShipment == null) return NotFound();

            // 2. İşlemi yapan kişinin kimliğini al
            var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier || c.Type == "sub");
            int.TryParse(userIdClaim?.Value, out int currentUserId);

            // 3. Admin mi kontrol et
            bool isAdmin = User.IsInRole("Admin");  // 👑 PATRON KONTROLÜ

            // 4. GÜVENLİK FİLTRESİ: Admin değilse ve kargo ona ait değilse YASAKLA!
            if (!isAdmin && existingShipment.AppUserId != currentUserId)
            {
                return StatusCode(403);
            }

            // Kargonun asıl sahibini koru (Başka personel güncellese bile ilk ekleyenin ID'si değişmesin)
            shipmentUpdateDto.AppUserId = existingShipment.AppUserId;

            await _shipmentService.UpdateAsync(shipmentUpdateDto);
            return Ok();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            // 1. Kargonun var olup olmadığını kontrol et
            var shipment = await _shipmentService.GetByIdAsync(id);
            if (shipment == null)
            {
                return NotFound();
            }

            // 2. İşlemi yapan kişinin kimliğini al
            var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier || c.Type == "sub");
            int.TryParse(userIdClaim?.Value, out int currentUserId);

            // 3. Admin mi kontrol et
            bool isAdmin = User.IsInRole("Admin");

            // 4. GÜVENLİK FİLTRESİ: Admin değilse ve kargo ona ait değilse YASAKLA!
            if (!isAdmin && shipment.AppUserId != currentUserId)
            {
                return StatusCode(403);
            }

            await _shipmentService.DeleteAsync(id);
            return Ok();
        }

        [HttpGet("GetByPersonnel/{id}")]
        [Authorize(Roles = "Admin")] // Sadece Admin'in görmesi güvenli olur
        public async Task<IActionResult> GetByPersonnel(int id)
        {
            // Tüm kargoları çekip ilgili personelin AppUserId'si ile eşleşenleri filtreliyoruz
            var allShipments = await _shipmentService.GetListAsync();
            var personnelShipments = allShipments.Where(s => s.AppUserId == id).ToList();

            return Ok(personnelShipments);
        }
    }
}