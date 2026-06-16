using ShipmentTracking.Business.Abstract;
using ShipmentTracking.DataAccess.Abstract;
using ShipmentTracking.Entities.Concrete;

namespace ShipmentTracking.Business.Concrete
{
    // HATA BURADAYDI: IShipmentHistoryRepository yerine IShipmentHistoryService yazmalısın!
    public class ShipmentHistoryManager : IShipmentHistoryService
    {
        private readonly IShipmentHistoryRepository _shipmentHistoryRepository;

        public ShipmentHistoryManager(IShipmentHistoryRepository shipmentHistoryRepository)
        {
            _shipmentHistoryRepository = shipmentHistoryRepository;
        }

        public async Task AddAsync(ShipmentHistory entity)
        {
            await _shipmentHistoryRepository.AddAsync(entity);
        }

        // HATA BURADAYDI: Interface'de "UpdateAsync" demiştin, burada da öyle olmalı.
        public async Task UpdateAsync(ShipmentHistory entity)
        {
            _shipmentHistoryRepository.Update(entity);
        }

        // HATA BURADAYDI: Interface'de "DeleteAsync" demiştin, burada da öyle olmalı.
        public async Task DeleteAsync(ShipmentHistory entity)
        {
            _shipmentHistoryRepository.Delete(entity);
        }

        public async Task<List<ShipmentHistory>> GetAllAsync()
        {
            return await _shipmentHistoryRepository.GetAllAsync();
        }

        public async Task<ShipmentHistory?> GetByIdAsync(int id)
        {
            return await _shipmentHistoryRepository.GetByIdAsync(id);
        }
    }
}