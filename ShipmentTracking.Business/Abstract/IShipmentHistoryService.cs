using ShipmentTracking.Entities.Concrete;
using System;
using System.Collections.Generic;
using System.Text;

namespace ShipmentTracking.Business.Abstract
{
    public interface IShipmentHistoryService
    {
        Task<List<ShipmentHistory>> GetAllAsync();
        Task<ShipmentHistory?> GetByIdAsync(int id);
        Task AddAsync(ShipmentHistory shipmentHistory);

        // Bunları da "Task" ile uyumlu hale getirelim:
        Task UpdateAsync(ShipmentHistory shipmentHistory);
        Task DeleteAsync(ShipmentHistory shipmentHistory);
    }
}
