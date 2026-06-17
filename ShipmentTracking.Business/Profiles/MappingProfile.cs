using AutoMapper;
using ShipmentTracking.Entities.Concrete;
using ShipmentTracking.Entities.DTOs;
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
        }
    }
}
