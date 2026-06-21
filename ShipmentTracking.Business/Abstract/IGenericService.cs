using System;
using System.Collections.Generic;
using System.Text;

namespace ShipmentTracking.Business.Abstract
{
    public interface IGenericService<T> where T : class
    {
        Task<List<T>> GetAllAsync();
        Task<T?> GetByIdAsync(int id);
        Task AddAsync(T entity);
        void Update(T entity);
        void Delete(T entity);
    }
}
