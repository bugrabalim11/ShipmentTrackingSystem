using Microsoft.EntityFrameworkCore;
using ShipmentTracking.Entities.Concrete;
using System;
using System.Collections.Generic;
using System.Text;

namespace ShipmentTracking.DataAccess.Concrete.EntityFramework
{
    public class AppDbContext : DbContext
    {
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            // Buraya bağlantı metnini yazacağız. 
            // Şimdilik buraya sabit kodluyoruz ama profesyonel projede bunu 'appsettings.json'dan okuyacağız.
            optionsBuilder.UseNpgsql("Host=localhost;Database=ShipmentDb;Username=postgres;Password=4596+17");
        }

        public DbSet<Shipment> Shipments { get; set; }
        public DbSet<ShipmentHistory> ShipmentHistories { get; set; }
    }
}
