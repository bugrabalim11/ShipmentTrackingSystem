using ShipmentTracking.DataAccess.Abstract;
using ShipmentTracking.Entities.Concrete;
using System;
using System.Collections.Generic;
using System.Text;

namespace ShipmentTracking.DataAccess.Concrete.EntityFramework
{
    public class ShipmentHistoryRepository : EfGenericRepository<ShipmentHistory>, IShipmentHistoryRepository
    {
        public ShipmentHistoryRepository(AppDbContext context) : base(context)
        {
        }
    }
}
