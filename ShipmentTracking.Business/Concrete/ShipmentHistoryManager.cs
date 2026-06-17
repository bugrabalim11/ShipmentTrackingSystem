using AutoMapper;
using ShipmentTracking.Business.Abstract;
using ShipmentTracking.DataAccess.Abstract;
using ShipmentTracking.Entities.Concrete;
using ShipmentTracking.Entities.DTOs.ShipmentHistory;

namespace ShipmentTracking.Business.Concrete
{
    // HATA BURADAYDI: IShipmentHistoryRepository yerine IShipmentHistoryService yazmalısın!
    public class ShipmentHistoryManager : IShipmentHistoryService
    {
        private readonly IShipmentHistoryRepository _shipmentHistoryRepository;
        private readonly IShipmentRepository _shipmentRepository; // 1. Ana kargo veritabanını içeri alıyoruz
        private readonly IMapper _mapper;

        // 2. Constructor'ı güncelliyoruz
        public ShipmentHistoryManager(IShipmentHistoryRepository shipmentHistoryRepository, IShipmentRepository shipmentRepository, IMapper mapper)
        {
            _shipmentHistoryRepository = shipmentHistoryRepository;
            _shipmentRepository = shipmentRepository;
            _mapper = mapper;
        }

        // 3. Ekleme metoduna o harika iş kuralını yazıyoruz
        public async Task AddAsync(ShipmentHistoryCreateDto shipmentHistoryCreateDto)
        {
            var history = _mapper.Map<ShipmentHistory>(shipmentHistoryCreateDto);
            history.ChangeDate = DateTime.UtcNow;

            // Önce geçmişi (History) veritabanına ekliyoruz
            await _shipmentHistoryRepository.AddAsync(history);

            // ---> İŞTE SENİN FARK ETTİĞİN O SİHİRLİ KURAL <---
            // Ana kargoyu ID'sinden bul
            var shipment = await _shipmentRepository.GetByIdAsync(shipmentHistoryCreateDto.ShipmentId);
            if (shipment != null)
            {
                // Ana kargonun durumunu, yeni eklenen geçmişin durumuyla ez (güncelle)
                shipment.Status = shipmentHistoryCreateDto.StatusDescription;

                // Ana kargoyu da veritabanında güncelle
                _shipmentRepository.Update(shipment);
            }
        }

        // HATA BURADAYDI: Interface'de "UpdateAsync" demiştin, burada da öyle olmalı.
        public async Task UpdateAsync(ShipmentHistoryUpdateDto shipmentHistoryUpdateDto)
        {
            var existingHistory = await _shipmentHistoryRepository.GetByIdAsync(shipmentHistoryUpdateDto.Id);
            if (existingHistory == null)
            {
                throw new KeyNotFoundException(); // Düz metin yok, standart C# hatası var!
            }

            _mapper.Map(shipmentHistoryUpdateDto, existingHistory);

            // EF Core'da Update metodu hafızada işlem yaptığı için Async değildir, normal çağırıyoruz
            _shipmentHistoryRepository.Update(existingHistory);
        }

        // HATA BURADAYDI: Interface'de "DeleteAsync" demiştin, burada da öyle olmalı.
        public async Task DeleteAsync(int id)
        {
            var existingHistory = await _shipmentHistoryRepository.GetByIdAsync(id);
            if (existingHistory != null) // != null boş değerine eşit değilse demek
            {
                _shipmentHistoryRepository.Delete(existingHistory);
            }
        }

        public async Task<List<ShipmentHistoryListDto>> GetAllAsync()
        {
            var histories = await _shipmentHistoryRepository.GetAllAsync();
            return _mapper.Map<List<ShipmentHistoryListDto>>(histories);
        }

        public async Task<ShipmentHistoryListDto?> GetByIdAsync(int id)
        {
            var history = await _shipmentHistoryRepository.GetByIdAsync(id);
            if (history == null) return null;

            return _mapper.Map<ShipmentHistoryListDto>(history);
        }
    }
}