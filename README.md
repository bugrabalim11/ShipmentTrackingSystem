📦 Kargo Takip Sistemi (Shipment Tracking System)

Bu proje, kargo süreçlerinin baştan sona yönetilebildiği, rol bazlı yetkilendirmeye sahip (Admin/Personel) ve N-Tier (Çok Katmanlı) Mimari prensiplerine uygun olarak geliştirilmiş kapsamlı bir Kargo Takip Sistemidir.

Proje, bağımsız bir RESTful API arka ucu (Backend) ve bu API ile haberleşen bir ASP.NET Core MVC ön yüzünden (Frontend) oluşmaktadır.

## 📸 Ekran Görüntüleri

| Ana Sayfa / Sorgula | Giriş Ekranı | Kargoları Yönet | Kargo Detay |
| :---: | :---: | :---: | :---: |
| ![Ana Sayfa](https://github.com/user-attachments/assets/a3256d32-9f9a-4c5f-8f2d-5ff7aa258cff) | ![Giriş](https://github.com/user-attachments/assets/f3d75577-b5fa-419f-8568-6c0bfd8a52a9) | ![Yönet](https://github.com/user-attachments/assets/34d439f1-b037-482d-af16-90b22ffbc39d) | ![Detay](https://github.com/user-attachments/assets/a47ab764-2b37-427d-8fdf-31a59704bac9) |

| Kargo Güncelle | Yeni Kargo Ekle | Personel Yönetimi | İşlem Geçmişi |
| :---: | :---: | :---: | :---: |
| ![Güncelle](https://github.com/user-attachments/assets/0337744e-a24a-45e9-8feb-2856b0387608) | ![Ekle](https://github.com/user-attachments/assets/24fe7575-a350-4ead-ae4f-52ec8c3fb2d9) | ![Personel](https://github.com/user-attachments/assets/f8633e0b-3334-4964-ac22-d20d10d8eed4) | ![Geçmiş](https://github.com/user-attachments/assets/72661fa9-72a0-492d-953c-2ce49ec58982) |

| Personel Düzenle | Yeni Personel Kaydı | Ana Sayfa/Kargo Sorgula |
| :---: | :---: | :---: |
| ![Düzenle](https://github.com/user-attachments/assets/ab9d5c43-c065-4a64-98b2-698717cc8544) | ![Kayıt](https://github.com/user-attachments/assets/696fde5a-58a5-4f76-993e-e37ab98d360d) | ![Ana Sayfa/Kargo Sorgula](https://github.com/user-attachments/assets/d1a57332-3524-4dd6-aa60-d44e619fac5c) |

🚀 Proje Özellikleri

- Kimlik Doğrulama & Yetkilendirme (Auth): JWT (JSON Web Token) tabanlı güvenli giriş sistemi. Kullanıcı şifreleri BCrypt ile şifrelenerek veritabanında saklanmaktadır.

- Rol Yönetimi (RBAC): Admin ve Personel olmak üzere iki farklı yetki seviyesi.

- Personel Yönetimi (Sadece Admin): Sisteme yeni personel ekleme, bilgileri (ad, soyad, rol) güncelleme, silme ve personelin yaptığı kargo işlemlerinin geçmişini görüntüleme.

- Kargo Yönetimi: Sisteme yeni kargo ekleme, kargo bilgilerini güncelleme ve silme.

- Kargo Hareket (İşlem) Geçmişi: Kargoların durumlarını (Örn: "Şubeye Ulaştı", "Dağıtıma Çıktı") tarih bazlı olarak sisteme işleme ve detay sayfasında listeleme.

- Halka Açık Kargo Sorgulama: Sisteme giriş yapmayan (misafir) kullanıcıların takip numarası ile kargolarının durumunu sorgulayabileceği public arayüz.

🛠️ Kullanılan Teknolojiler ve Mimari

Bu proje, "Clean Code" ve ayrık bileşen (Separation of Concerns) prensipleri göz önünde bulundurularak geliştirilmiştir.

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

- Rol -> Admin -> Personel

- Kullanıcı Adı -> superadmin -> bugra11

- Şifre -> Password -> Bugra123

- Yetkiler -> Personel yönetimi (Ekle/Sil/Düzenle), tüm kargo işlemleri. -> Kargo ekleme, kargo durumu güncelleme. Personel paneline erişemez.


⚙️ Kurulum ve Çalıştırma

1- Projeyi bilgisayarınıza klonlayın:

git clone https://github.com/kullaniciadiniz/ShipmentTrackingSystem.git

2- ShipmentTracking.API projesi içindeki appsettings.json dosyasını açın ve ConnectionStrings kısmını kendi SQL Server / PostgreSQL veritabanınıza göre güncelleyin.

3- Package Manager Console (PMC) üzerinden API projesini seçerek veritabanını oluşturun:

Update-Database

4- Çözüme (Solution) sağ tıklayıp "Set Startup Projects" seçeneğine gidin. Hem API hem de WebUI projelerinin "Start" olarak ayarlandığından emin olun (Multiple startup projects).

5- Projeyi başlatın.
💡 Geliştirici Notu

"Benim adım Buğra. Yönetim Bilişim Sistemleri (YBS) 2. sınıf öğrencisiyim. Bu proje, sadece eğitim videolarını izleyerek değil; arka plandaki yazılım mimarisini, N-Tier yapılarını, Clean Code prensiplerini ve 'Best Practice' standartlarını sorgulayarak, yapay zeka tabanlı Senior Kodlama Mentorüm (Gemini) ile omuz omuza, interaktif bir şekilde geliştirilmiştir. Kopyala-yapıştır yapmaktan ziyade, sistemin nasıl çalıştığını (Under the hood) anlayarak kodlanmıştır."
