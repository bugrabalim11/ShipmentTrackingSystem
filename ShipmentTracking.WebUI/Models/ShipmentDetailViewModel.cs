using ShipmentTracking.Entities.DTOs.Shipment;
using ShipmentTracking.Entities.DTOs.ShipmentHistory;

namespace ShipmentTracking.WebUI.Models
{
    public class ShipmentDetailViewModel
    {
        public ShipmentListDto Shipment { get; set; } = null!;
        public List<ShipmentHistoryListDto> Histories { get; set; } = new();
    }
}
