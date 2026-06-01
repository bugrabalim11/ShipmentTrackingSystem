using System;
using System.Collections.Generic;
using System.Text;

namespace ShipmentTracking.Entities.Concrete
{
    public class ShipmentHistory
    {
        public int Id { get; set; }

        // Bu kayıt hangi kargoya ait?
        public int ShipmentId { get; set; }

        // Durum neydi? (Örn: "Şubeye Ulaştı")
        public string StatusDescription { get; set; } = string.Empty;

        // Bu durum ne zaman gerçekleşti?
        public DateTime ChangeDate { get; set; }

        // Hangi şehirde/lokasyonda yaşandı?
        public string Location { get; set; } = string.Empty;

        // NAVIGATION PROPERTY: 
        // EF Core'a "Bir History bir Shipment'a aittir" diyoruz.
        public Shipment? Shipment { get; set; }
    }
}
