using System;
using System.Collections.Generic;
using System.Text;

namespace ShipmentTracking.Entities.DTOs.ShipmentHistory
{
    public class ShipmentHistoryUpdateDto
    {
        public int Id { get; set; } // Güncellenecek hareketin ID'si şart!
        public int ShipmentId { get; set; }
        public string StatusDescription { get; set; } = string.Empty;
        public string Location { get; set; } = string.Empty;
    }
}
