using ShipmentTracking.Entities.Concrete;
using System;
using System.Collections.Generic;
using System.Text;

namespace ShipmentTracking.Business.Abstract
{
    // API'nin kullanacağı kargo metotlarının listesi
    public interface IShipmentService
    {
        Task<List<Shipment>> GetListAsync(); // Zaten Async
        Task<Shipment?> GetByIdAsync(int id); // Zaten Async  ? null dönebilir demek için ekledik
        Task AddAsync(Shipment shipment);     // Zaten Async

        // Geriye kalanları da "Task" yapıp "Async" ekliyoruz:
        Task UpdateAsync(Shipment shipment);
        Task DeleteAsync(Shipment shipment);


        // DataAccess'te yazdığımız özel metodu buraya da ekliyoruz ki API kullanabilsin
        Task<Shipment?> GetShipmentWithHistoryAsync(int id);
    }
}
