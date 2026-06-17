using System;
using System.Collections.Generic;
using System.Text;

namespace ShipmentTracking.Entities.DTOs.Shipment
{
    public class ShipmentListDto
    {
        public int Id { get; set; }
        public string TrackingNumber { get; set; } = String.Empty;
        public string SenderName { get; set; } = String.Empty;
        public string ReceiverName { get; set; } = String.Empty;
        public string Status { get; set; } = String.Empty;
        public DateTime CreatedDate { get; set; }
    }
}
