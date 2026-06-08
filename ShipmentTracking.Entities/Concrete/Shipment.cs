using System;
using System.Collections.Generic;
using System.Text;

namespace ShipmentTracking.Entities.Concrete
{
    public class Shipment
    {
        // "Id" ismi EF Core için özeldir. Otomatik "Primary Key" yani eşsiz kimlik olur.
        public int Id { get; set; }

        // Takip numarası benzersiz olmalı (Unique). 
        // İleride bunu veritabanı seviyesinde de "Unique" yapmayı öğreneceğiz.
        public string TrackingNumber { get; set; } = string.Empty;

        public string SenderName { get; set; } = string.Empty;
        public string ReceiverName { get; set; } = string.Empty;

        // Kargonun şu anki durumu
        public string Status { get; set; } = string.Empty;

        // Kargonun oluşturulma zamanı
        public DateTime CreatedDate { get; set; }

        // YENİ EKLENEN KISIM: Bire-Çok İlişki (Bir kargonun birden fazla geçmişi olur)
        public List<ShipmentHistory> ShipmentHistories { get; set; } = new List<ShipmentHistory>();
    }
}
