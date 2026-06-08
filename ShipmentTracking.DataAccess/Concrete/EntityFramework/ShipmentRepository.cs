using Microsoft.EntityFrameworkCore;
using ShipmentTracking.DataAccess.Abstract;
using ShipmentTracking.Entities.Concrete;
using System;
using System.Collections.Generic;
using System.Text;

namespace ShipmentTracking.DataAccess.Concrete.EntityFramework
{
    // Hem EfGenericRepository'deki hazır metodları alıyor, hem de IShipmentRepository'deki özel metodun sözünü tutuyor.
    public class ShipmentRepository : EfGenericRepository<Shipment>, IShipmentRepository
    {
        // Temel sınıfın (EfGenericRepository) AppDbContext bekleyen constructor'ına (yapıcı metoduna) veritabanını gönderiyoruz.
        public ShipmentRepository(AppDbContext context) : base(context)
        {
        }


        // IShipmentRepository'de söz verdiğimiz o özel metodu burada dolduruyoruz.
        public async Task<Shipment?> GetShipmentWithHistoryAsync(int id)
        {
            // Include() metodu EF Core'un en güçlü silahıdır. (Eager Loading)
            // Kargo bilgisini getirirken, ilişkili olduğu kargo geçmişini (ShipmentHistories) de SQL'de JOIN atarak tek seferde çeker.
            return await _context.Shipments
                                 .Include(s => s.ShipmentHistories)
                                 .FirstOrDefaultAsync(s => s.Id == id);
        }
    }
}
