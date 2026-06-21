using ShipmentTracking.Business.Abstract;
using ShipmentTracking.DataAccess.Abstract;
using ShipmentTracking.Entities.Concrete;
using System;
using System.Collections.Generic;
using System.Text;

namespace ShipmentTracking.Business.Concrete
{
    public class AppUserManager : IAppUserService
    {
        private readonly IAppUserRepository _appUserRepository;

        public AppUserManager(IAppUserRepository appUserRepository)
        {
            _appUserRepository = appUserRepository;
        }

        public async Task AddAsync(AppUser entity)
        {
            await _appUserRepository.AddAsync(entity);
        }

        public void Delete(AppUser entity)
        {
            _appUserRepository.Delete(entity);
        }

        public async Task<List<AppUser>> GetAllAsync()
        {
            return await _appUserRepository.GetAllAsync();
        }

        public async Task<AppUser?> GetByIdAsync(int id)
        {
            return await _appUserRepository.GetByIdAsync(id);
        }

        public void Update(AppUser entity)
        {
            _appUserRepository.Update(entity);
        }
    }
}
