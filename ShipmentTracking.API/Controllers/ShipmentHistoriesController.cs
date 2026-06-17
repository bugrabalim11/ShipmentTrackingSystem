using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ShipmentTracking.Business.Abstract;
using ShipmentTracking.Entities.Concrete;

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
            var result=await _shipmentHistoryService.GetAllAsync();
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> Add(ShipmentHistory shipmentHistory)
        {
            await _shipmentHistoryService.AddAsync(shipmentHistory);
            return StatusCode(201);
        }
    }
}
