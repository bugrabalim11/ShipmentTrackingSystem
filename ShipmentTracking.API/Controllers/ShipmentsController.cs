using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ShipmentTracking.Business.Abstract;
using ShipmentTracking.Entities.Concrete;
using ShipmentTracking.Entities.DTOs.Shipment;

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

        [HttpPost]
        public async Task<IActionResult> Add(ShipmentCreateDto shipmentCreateDto)
        {
            await _shipmentService.AddAsync(shipmentCreateDto);
            return StatusCode(201);
        }

        [HttpPut]
        public async Task<IActionResult> Update(ShipmentUpdateDto shipmentUpdateDto)
        {
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

            await _shipmentService.DeleteAsync(id);
            return Ok();
        }
    }
}
