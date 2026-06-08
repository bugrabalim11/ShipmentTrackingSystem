using ShipmentTracking.Entities.Concrete;
using System;
using System.Collections.Generic;
using System.Text;

namespace ShipmentTracking.DataAccess.Abstract
{
    public interface IShipmentHistoryRepository: IGenericRepository<ShipmentHistory>
    {
        // Sadece Kargo geçmişine özel olan, Generic yapıda OLMAYAN ekstra metodları buraya yazarız.
    }
}
