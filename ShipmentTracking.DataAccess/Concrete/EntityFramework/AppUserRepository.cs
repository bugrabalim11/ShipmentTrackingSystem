using ShipmentTracking.DataAccess.Abstract;
using ShipmentTracking.Entities.Concrete;
using System;
using System.Collections.Generic;
using System.Text;

namespace ShipmentTracking.DataAccess.Concrete.EntityFramework
{
    public class AppUserRepository : EfGenericRepository<AppUser>, IAppUserRepository
    {
        public AppUserRepository(AppDbContext context) : base(context)
        {
        }
    }
}
