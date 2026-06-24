using System;
using System.Collections.Generic;
using System.Text;

namespace ShipmentTracking.Entities.DTOs.Shipment
{
    public class ShipmentCreateDto
    {
        public string TrackingNumber { get; set; } = string.Empty;
        public string SenderName { get; set; } = string.Empty;
        public string ReceiverName { get; set; } = string.Empty;

        // Status (Durum) ve CreatedDate (Oluşturulma Tarihi) istemiyoruz, çünkü
        // yeni kargonun durumu varsayılan olarak "Hazırlanıyor" olacak ve tarihi sistem o an atayacak.

        public int AppUserId { get; set; }
    }
}
