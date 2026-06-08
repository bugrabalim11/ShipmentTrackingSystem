using Microsoft.EntityFrameworkCore;
using ShipmentTracking.DataAccess.Abstract;
using System;
using System.Collections.Generic;
using System.Text;

namespace ShipmentTracking.DataAccess.Concrete.EntityFramework
{
    // Bu sınıf, yazdığımız kuralları (IGenericRepository) gerçekten işleyecek olan sınıftır.
    public class EfGenericRepository<T> : IGenericRepository<T> where T : class
    {
        protected readonly AppDbContext _context;

        // Dependency Injection (Bağımlılık Enjeksiyonu) ile DbContext'imizi içeri alıyoruz.
        public EfGenericRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(T entity)
        {
            // Set<T> ifadesi, T hangi tabloysa (Shipment veya ShipmentHistory) o tabloya odaklanır.
            await _context.Set<T>().AddAsync(entity);
            await _context.SaveChangesAsync();
        }

        public void Delete(T entity)
        {
            _context.Set<T>().Remove(entity);
            _context.SaveChanges();
        }

        public async Task<List<T>> GetAllAsync()
        {
            return await _context.Set<T>().ToListAsync();
        }

        public Task<T?> GetByIdAsync(int id)
        {
            return _context.Set<T>().FindAsync(id).AsTask();
        }

        public void Update(T entity)
        {
            _context.Set<T>().Update(entity);
            _context.SaveChanges();
        }
    }
}
