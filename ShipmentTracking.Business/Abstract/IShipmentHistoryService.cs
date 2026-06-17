using ShipmentTracking.Entities.Concrete;
using ShipmentTracking.Entities.DTOs.Shipment;
using ShipmentTracking.Entities.DTOs.ShipmentHistory;
using System;
using System.Collections.Generic;
using System.Text;

namespace ShipmentTracking.Business.Abstract
{
    public interface IShipmentHistoryService
    {
        Task<List<ShipmentHistoryListDto>> GetAllAsync();
        Task<ShipmentHistoryListDto?> GetByIdAsync(int id);
        Task AddAsync(ShipmentHistoryCreateDto shipmentHistoryCreateDto);

        // Bunları da "Task" ile uyumlu hale getirelim:
        Task UpdateAsync(ShipmentHistoryUpdateDto shipmentHistoryUpdateDto);
        Task DeleteAsync(int id);
    }
}
