using AutoMapper;
using ShipmentTracking.Entities.Concrete;
using ShipmentTracking.Entities.DTOs.Auth;
using ShipmentTracking.Entities.DTOs.Shipment;
using ShipmentTracking.Entities.DTOs.ShipmentHistory;
using System;
using System.Collections.Generic;
using System.Text;

namespace ShipmentTracking.Business.Profiles
{
    // Profile sınıfından miras alması AutoMapper'ın burayı tanımasını sağlar
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // GET İşlemi için: Veritabanı nesnesini (Shipment) -> Vitrin formuna (ShipmentListDto) çevir
            CreateMap<Shipment, ShipmentListDto>();

            // POST İşlemi için: Kullanıcının doldurduğu formu (ShipmentCreateDto) -> Veritabanı nesnesine (Shipment) çevir
            CreateMap<ShipmentCreateDto, Shipment>();

            // ---> YENİ EKLENDİ <---
            // Güncelleme işlemi için: Formu al, var olan veritabanı nesnesinin üzerine yaz
            CreateMap<ShipmentUpdateDto, Shipment>();


            // ---> YENİ EKLENEN KISIM (ShipmentHistory) <---
            CreateMap<ShipmentHistory, ShipmentHistoryListDto>();
            CreateMap<ShipmentHistoryCreateDto, ShipmentHistory>();
            CreateMap<ShipmentHistoryUpdateDto, ShipmentHistory>();

            // AutoMapper'a Bu İki Sınıfı Tanıştır
            CreateMap<AppUser, UserResponseDto>();
        }
    }
}
