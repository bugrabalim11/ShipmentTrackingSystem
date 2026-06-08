using ShipmentTracking.Entities.Concrete;
using System;
using System.Collections.Generic;
using System.Text;

namespace ShipmentTracking.DataAccess.Abstarct
{
    // IGenericRepository'nin tüm yeteneklerini Shipment sınıfı için miras alıyoruz.
    public interface IShipmentRepository : IGenericRepository<Shipment>
    {
        // Sadece Kargoya özel olan, Generic yapıda OLMAYAN ekstra metodları buraya yazarız.

        // Örnek: Kargoyu, geçmiş hareketleriyle (ShipmentHistories) birlikte getiren özel bir metod.
        Task<Shipment?> GetShipmentWithHistoryAsync(int id);
    }
}
