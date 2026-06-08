using System;
using System.Collections.Generic;
using System.Text;

namespace ShipmentTracking.DataAccess.Abstarct
{
    // 'T' harfi, bu interface'in her tür sınıf (Entity) ile çalışabileceğini söyler.
    // 'where T : class' kısıtlaması ise bu 'T'nin sadece bir sınıf (yani bizim Shipment, ShipmentHistory gibi modellerimiz) olabileceğini garanti eder.
    public interface IGenericRepository<T> where T : class
    {
        Task<List<T>> GetAllAsync();
        Task<T?> GetByIdAsync(int id);
        Task AddAsync(T entity);
        void Update(T entity);
        void Delete(T entity);
    }
}
