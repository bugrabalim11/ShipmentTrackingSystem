using System;
using System.Collections.Generic;
using System.Text;

namespace ShipmentTracking.Entities.DTOs
{
    public class ShipmentUpdateDto
    {
        public int Id { get; set; } // Güncellemede ID şart! Hangi kargoyu güncellediğimizi bilmeliyiz.
        public string SenderName { get; set; } = String.Empty;
        public string ReceiverName { get; set; } = String.Empty;
        public string TrackingNumber { get; set; } = String.Empty;
        public string Status { get; set; } = String.Empty; // Güncellemede durumu da değiştirebiliriz
    }
}
