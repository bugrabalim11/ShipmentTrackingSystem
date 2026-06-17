using ShipmentTracking.Entities.Concrete;
using ShipmentTracking.Entities.DTOs.Shipment;
using System;
using System.Collections.Generic;
using System.Text;

namespace ShipmentTracking.Business.Abstract
{
    // API'nin kullanacağı kargo metotlarının listesi
    public interface IShipmentService
    {
        // 1. Tüm listeyi getirirken vitrin formunu (ShipmentListDto) kullan
        Task<List<ShipmentListDto>> GetListAsync(); // Zaten Async

        // 2. Tek bir kargo getirirken yine vitrin formunu kullan
        Task<ShipmentListDto?> GetByIdAsync(int id); // ? null dönebilir demek için ekledik

        // 3. Eklerken sadece kullanıcının girdiği "Ekleme" formunu al
        Task AddAsync(ShipmentCreateDto shipmentCreateDto);

        // Geriye kalanları da "Task" yapıp "Async" ekliyoruz
        // 4. Güncellerken içinde ID'si ve Status'ü olan "Güncelleme" formunu al
        Task UpdateAsync(ShipmentUpdateDto shipmentUpdateDto);

        // 5. Silerken devasa nesneyi taşımaya gerek yok, sadece ID'sini bilmemiz yeterli!
        Task DeleteAsync(int id);


        // DataAccess'te yazdığımız özel metodu buraya da ekliyoruz ki API kullanabilsin
        // Şimdilik bunu ham haliyle bırakabiliriz, DTO mantığını oturttuktan sonra buna özel DTO da yazarız
        Task<Shipment?> GetShipmentWithHistoryAsync(int id);
    }
}
