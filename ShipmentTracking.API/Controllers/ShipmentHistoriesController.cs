using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ShipmentTracking.Business.Abstract;
using ShipmentTracking.Entities.Concrete;
using ShipmentTracking.Entities.DTOs.ShipmentHistory;

namespace ShipmentTracking.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ShipmentHistoriesController : ControllerBase
    {
        private readonly IShipmentHistoryService _shipmentHistoryService;

        // Dependency Injection ile Servisimizi içeri alıyoruz
        public ShipmentHistoriesController(IShipmentHistoryService shipmentHistoryService)
        {
            _shipmentHistoryService = shipmentHistoryService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _shipmentHistoryService.GetAllAsync();
            return Ok(result); // Sadece 200 OK ve Veri
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _shipmentHistoryService.GetByIdAsync(id);
            if (result == null) return NotFound(); // 404 Not Found
            
            return Ok(result); // 200 OK ve Veri
        }

        [HttpPost]
        public async Task<IActionResult> Add(ShipmentHistoryCreateDto shipmentHistoryCreateDto)
        {
            await _shipmentHistoryService.AddAsync(shipmentHistoryCreateDto);
            return StatusCode(201);
        }

        [HttpPut]
        public async Task<IActionResult> Update(ShipmentHistoryUpdateDto shipmentHistoryUpdateDto)
        {
            await _shipmentHistoryService.UpdateAsync(shipmentHistoryUpdateDto);
            return NoContent(); // Başarılı ama ekranda gösterilecek yeni veri yok (204 No Content - Tam profesyonel standart)
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _shipmentHistoryService.DeleteAsync(id);
            return NoContent();
        }
    }
}
