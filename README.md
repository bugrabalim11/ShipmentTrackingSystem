📦 Kargo Takip Sistemi (Shipment Tracking System)

Bu proje, kargo süreçlerinin baştan sona yönetilebildiği, rol bazlı yetkilendirmeye sahip (Admin/Personel) ve N-Tier (Çok Katmanlı) Mimari prensiplerine uygun olarak geliştirilmiş kapsamlı bir Kargo Takip Sistemidir.

Proje, bağımsız bir RESTful API arka ucu (Backend) ve bu API ile haberleşen bir ASP.NET Core MVC ön yüzünden (Frontend) oluşmaktadır.

🚀 Proje Özellikleri

- Kimlik Doğrulama & Yetkilendirme (Auth): JWT (JSON Web Token) tabanlı güvenli giriş sistemi. Kullanıcı şifreleri BCrypt ile şifrelenerek veritabanında saklanmaktadır.

- Rol Yönetimi (RBAC): Admin ve Personel olmak üzere iki farklı yetki seviyesi.

- Personel Yönetimi (Sadece Admin): Sisteme yeni personel ekleme, bilgileri (ad, soyad, rol) güncelleme, silme ve personelin yaptığı kargo işlemlerinin geçmişini görüntüleme.

- Kargo Yönetimi: Sisteme yeni kargo ekleme, kargo bilgilerini güncelleme ve silme.

- Kargo Hareket (İşlem) Geçmişi: Kargoların durumlarını (Örn: "Şubeye Ulaştı", "Dağıtıma Çıktı") tarih bazlı olarak sisteme işleme ve detay sayfasında listeleme.

- Halka Açık Kargo Sorgulama: Sisteme giriş yapmayan (misafir) kullanıcıların takip numarası ile kargolarının durumunu sorgulayabileceği public arayüz.

🛠️ Kullanılan Teknolojiler ve Mimari

- Bu proje, "Clean Code" ve ayrık bileşen (Separation of Concerns) prensipleri göz önünde bulundurularak geliştirilmiştir.

Backend (ShipmentTracking.API & Data/Business Katmanları)

- Framework: ASP.NET Core Web API

- ORM: Entity Framework Core

- Veritabanı: PostgreSQL

- Mimari: N-Tier Architecture (Entities, DataAccess, Business, API) ve Generic Repository Pattern

- Güvenlik: JWT (JSON Web Token) & BCrypt.Net Password Hashing

- Veri Transferi (DTO): AutoMapper ile DTO (Data Transfer Object) entegrasyonu.

Frontend (ShipmentTracking.WebUI)

- Framework: ASP.NET Core MVC

- Http İstemcisi: HttpClient ile RESTful API entegrasyonu ve merkezi Token yönetimi (BaseController yaklaşımı).

- Arayüz (UI): HTML5, CSS3, Bootstrap 5

- Bildirimler: SweetAlert2 (Kullanıcı dostu toast bildirimleri)

🔐 Test Hesapları

Projeyi ayağa kaldırdıktan sonra sistemi test etmek için aşağıdaki hesapları kullanabilirsiniz:

Rol -> Admin -> Personel

Kullanıcı Adı -> superadmin -> bugra11

Şifre -> Password -> Bugra123

Yetkiler -> Personel yönetimi (Ekle/Sil/Düzenle), tüm kargo işlemleri. -> Kargo ekleme, kargo durumu güncelleme. Personel paneline erişemez.

⚙️ Kurulum ve Çalıştırma

1- Projeyi bilgisayarınıza klonlayın:

git clone https://github.com/kullaniciadiniz/ShipmentTrackingSystem.git


2- ShipmentTracking.API projesi içindeki appsettings.json dosyasını açın ve ConnectionStrings kısmını kendi SQL Server / PostgreSQL veritabanınıza göre güncelleyin.

3- Package Manager Console (PMC) üzerinden API projesini seçerek veritabanını oluşturun:

Update-Database

4- Çözüme (Solution) sağ tıklayıp "Set Startup Projects" seçeneğine gidin. Hem API hem de WebUI projelerinin "Start" olarak ayarlandığından emin olun (Multiple startup projects).

5- Projeyi başlatın.

💡 Geliştirici Notu

"Benim adım Buğra. Yönetim Bilişim Sistemleri (YBS) 1. sınıf öğrencisiyim. Bu proje, sadece eğitim videolarını izleyerek değil; arka plandaki yazılım mimarisini, N-Tier yapılarını, Clean Code prensiplerini ve 'Best Practice' standartlarını sorgulayarak, yapay zeka tabanlı Senior Kodlama Mentorüm (Gemini) ile omuz omuza, interaktif bir şekilde geliştirilmiştir. Kopyala-yapıştır yapmaktan ziyade, sistemin nasıl çalıştığını (Under the hood) anlayarak kodlanmıştır."
