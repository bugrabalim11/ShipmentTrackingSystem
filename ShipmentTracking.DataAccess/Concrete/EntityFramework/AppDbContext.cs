using Microsoft.EntityFrameworkCore;
using ShipmentTracking.Entities.Concrete;
using System;
using System.Collections.Generic;
using System.Text;

namespace ShipmentTracking.DataAccess.Concrete.EntityFramework
{
    public class AppDbContext : DbContext
    {
        // ---> EKLENECEK KISIM BURASI <---
        // API'den gelen ayarları (options) alıp, DbContext'in ana sınıfına (base) iletiyoruz.
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }
        public DbSet<Shipment> Shipments { get; set; }
        public DbSet<ShipmentHistory> ShipmentHistories { get; set; }
    }
}
