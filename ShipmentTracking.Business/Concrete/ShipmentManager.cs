using ShipmentTracking.Business.Abstract;
using ShipmentTracking.DataAccess.Abstract;
using ShipmentTracking.Entities.Concrete;
using System;
using System.Collections.Generic;
using System.Text;

namespace ShipmentTracking.Business.Concrete
{
    public class ShipmentManager : IShipmentService
    {
        // Business katmanı veritabanı ile değil, Veznedar (Repository) ile konuşur!
        private readonly IShipmentRepository _shipmentRepository;

        // Dependency Injection ile Repository'yi içeri alıyoruz
        public ShipmentManager(IShipmentRepository shipmentRepository)
        {
            _shipmentRepository = shipmentRepository;
        }

        public async Task AddAsync(Shipment shipment)
        {
            // İleride buraya "Takip numarası boş mu?" gibi kontrolleri ekleyeceğiz.
            await _shipmentRepository.AddAsync(shipment);
        }

        public async Task DeleteAsync(Shipment shipment)
        {
            _shipmentRepository.Delete(shipment);
        }

        public async Task<Shipment?> GetByIdAsync(int id)
        {
            return await _shipmentRepository.GetByIdAsync(id);
        }

        public async Task<List<Shipment>> GetListAsync()
        {
            return await _shipmentRepository.GetAllAsync();
        }

        public async Task<Shipment?> GetShipmentWithHistoryAsync(int id)
        {
            return await _shipmentRepository.GetShipmentWithHistoryAsync(id);
        }

        public async Task UpdateAsync(Shipment shipment)
        {
            _shipmentRepository.Update(shipment);
        }
    }
}
