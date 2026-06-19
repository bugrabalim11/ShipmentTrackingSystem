using System;
using System.Collections.Generic;
using System.Text;

namespace ShipmentTracking.Entities.Concrete
{
    public class AppUser
    {
        public int Id { get; set; }

        // 1. Ekranda "Hoş geldin" demek için kullanılacak gerçek Ad ve Soyad
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;

        // 2. Sisteme giriş yaparken (Login formunda) kullanılacak takma ad
        public string UserName { get; set; } = string.Empty;

        // Şifre (Şimdilik düz metin tutacağız, gerçek projelerde Hash'lenir)
        public string Password { get; set; } = string.Empty;
        public string Role { get; set; } = "Admin";
    }
}
