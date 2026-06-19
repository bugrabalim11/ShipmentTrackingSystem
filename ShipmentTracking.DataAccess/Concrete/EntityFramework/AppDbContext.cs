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
        public DbSet<AppUser> AppUsers { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Veritabanı güncellendiğinde içine varsayılan bir yönetici ekliyoruz
            modelBuilder.Entity<AppUser>().HasData(
                new AppUser
                {
                    Id = 1,
                    FirstName = "Sistem",
                    LastName = "Yöneticisi",
                    UserName = "admin",
                    Password = "123", 
                    Role = "Admin"
                }
            );
        }
    }
}
