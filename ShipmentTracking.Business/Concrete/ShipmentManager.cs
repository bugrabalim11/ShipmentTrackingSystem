using AutoMapper;
using ShipmentTracking.Business.Abstract;
using ShipmentTracking.DataAccess.Abstract;
using ShipmentTracking.Entities.Concrete;
using ShipmentTracking.Entities.DTOs.Shipment;
using System;
using System.Collections.Generic;
using System.Text;

namespace ShipmentTracking.Business.Concrete
{
    public class ShipmentManager : IShipmentService
    {
        // Business katmanı veritabanı ile değil, Veznedar (Repository) ile konuşur!
        private readonly IShipmentRepository _shipmentRepository;
        private readonly IMapper _mapper;

        // Dependency Injection ile Repository'yi içeri alıyoruz
        public ShipmentManager(IShipmentRepository shipmentRepository, IMapper mapper)
        {
            _shipmentRepository = shipmentRepository;
            _mapper = mapper;
        }

        public async Task AddAsync(ShipmentCreateDto shipmentCreateDto)
        {
            var shipment = _mapper.Map<Shipment>(shipmentCreateDto);

            // Kullanıcıdan gizlediğimiz alanları sistem otomatik dolduruyor
            shipment.Status = "Hazırlanıyor";
            shipment.CreatedDate = DateTime.UtcNow;

            await _shipmentRepository.AddAsync(shipment);
        }

        public async Task DeleteAsync(int id)
        {
            var shipment = await _shipmentRepository.GetByIdAsync(id);
            if(shipment != null)
            {
                // DEĞİŞEN KISIM: Repository'de DeleteAsync olmadığı için Delete kullanıyoruz
                _shipmentRepository.Delete(shipment);
            }
        }

        public async Task<ShipmentListDto?> GetByIdAsync(int id)
        {
            var shipment = await _shipmentRepository.GetByIdAsync(id);
            if(shipment == null) return null;

            return _mapper.Map<ShipmentListDto>(shipment);
        }

        public async Task<List<ShipmentListDto>> GetListAsync()
        {
            var shipments = await _shipmentRepository.GetAllAsync(); // Veritabanından asenkron çek
            return _mapper.Map<List<ShipmentListDto>>(shipments);    // Vitrin formuna çevir ve dön
        }

        // Özel Geçmişli Metot (Şimdilik ham bırakıyoruz demiştik)
        public async Task<Shipment?> GetShipmentWithHistoryAsync(int id)
        {
            return await _shipmentRepository.GetShipmentWithHistoryAsync(id);
        }

        public async Task UpdateAsync(ShipmentUpdateDto shipmentUpdateDto)
        {
            // Önce güncellenecek kargo gerçekten veritabanında var mı diye kontrol ediyoruz
            var existingShipment = await _shipmentRepository.GetByIdAsync(shipmentUpdateDto.Id);
            if (existingShipment == null)
            {
                throw new KeyNotFoundException(); // Düz metin yok, standart C# hatası var!
            }

            // AutoMapper Sihri: Kullanıcıdan gelen güncel form bilgilerini (shipmentUpdateDto), 
            // veritabanından çektiğimiz orijinal nesnenin (existingShipment) üzerine yazar.
            _mapper.Map(shipmentUpdateDto, existingShipment);

            // DEĞİŞEN KISIM: Repository'de UpdateAsync olmadığı için Update kullanıyoruz
            _shipmentRepository.Update(existingShipment);
        }
    }
}
