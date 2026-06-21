using ShipmentTracking.Business.Abstract;
using ShipmentTracking.Business.Concrete;
using ShipmentTracking.DataAccess.Abstract;
using ShipmentTracking.DataAccess.Concrete.EntityFramework;

namespace ShipmentTracking.API.Extensions
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddBusinessServices(this IServiceCollection services)
        {
            // Business ve DataAccess katmanlarımızı birbirine bağlıyoruz

            services.AddScoped<IShipmentRepository, ShipmentRepository>();
            services.AddScoped<IShipmentService, ShipmentManager>();

            services.AddScoped<IShipmentHistoryRepository, ShipmentHistoryRepository>();
            services.AddScoped<IShipmentHistoryService, ShipmentHistoryManager>();

            // --- EKSİK OLAN PARÇALAR (BUNLARI EKLE) ---
            services.AddScoped<IAppUserRepository, AppUserRepository>();
            services.AddScoped<IAppUserService, AppUserManager>();

            return services;
        }
    }
}
