using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using ShipmentTracking.Business.Abstract;
using ShipmentTracking.Entities.Concrete;
using ShipmentTracking.Entities.DTOs.Auth;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace ShipmentTracking.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        // ARTIK DBCONTEXT YOK! Kendi yazdığımız Servis var.
        private readonly IAppUserService _appUserService;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;  // appsettings'i okumak için eklendi

        public AuthController(IAppUserService appUserService, IMapper mapper, IConfiguration configuration)
        {
            _appUserService = appUserService;
            _mapper = mapper;
            _configuration = configuration;
        }

        [HttpPost("Login")]
        public async Task<IActionResult> Login(LoginDto loginDto)
        {
            // 1. Generic servisimizden tüm kullanıcıları çekiyoruz
            var users = await _appUserService.GetAllAsync();

            // 1. Önce sadece Kullanıcı Adına göre kişiyi buluyoruz (Şifreyi sormuyoruz daha!)
            var user = users.FirstOrDefault(x => x.UserName == loginDto.UserName);

            if (user == null)
            {
                return Unauthorized();
            }

            // 2. Kripto Doğrulaması: Kullanıcının girdiği "123" ile veritabanındaki karmaşık metni karşılaştırıyoruz
            bool isPasswordValid = BCrypt.Net.BCrypt.Verify(loginDto.Password, user.Password);

            if (!isPasswordValid)
            {
                return Unauthorized();
            }

            // --- 3. JWT TOKEN ÜRETİM SÜRECİ BAŞLIYOR ---

            // A. Kullanıcının Kimlik Kartı Bilgileri (Claims)
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.UniqueName, user.UserName),
                new Claim(ClaimTypes.GivenName, $"{user.FirstName} {user.LastName}"),
                new Claim(ClaimTypes.Role, user.Role)
            };

            // B. Gizli Anahtarı appsettings.json'dan okuyoruz
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            // C. Bileti (Token'ı) oluşturuyoruz
            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddHours(2), // Bilet 2 saat geçerli
                signingCredentials: creds
            );

            // D. Bileti string formata çeviriyoruz (eO.s/Q... şeklinde)
            var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

            // --- 4. ARTIK SADECE USER BİLGİSİ DEĞİL, TOKEN DA DÖNÜYORUZ ---

            // Kullanıcı bilgilerini ve üretilen Token'ı anonim bir obje ile dönüyoruz
            return Ok(new
            {
                Token = tokenString,
                User = _mapper.Map<UserResponseDto>(user)
            });
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("Register")]
        public async Task<IActionResult> Register(RegisterDto registerDto)
        {
            // 1. İş Kuralı: Aynı kullanıcı adından var mı?
            var users = await _appUserService.GetAllAsync();
            var anyUser = users.Any(x => x.UserName == registerDto.UserName);

            if (anyUser)
            {
                return BadRequest();
            }

            // 2. Sihir: DTO'yu AppUser nesnesine dönüştür
            var newUser = _mapper.Map<AppUser>(registerDto);

            // İŞTE SİBER GÜVENLİK BURADA BAŞLIYOR!
            // Kullanıcının "123" olarak girdiği şifreyi alıp geri döndürülemez bir kriptoya çeviriyoruz.
            newUser.Password = BCrypt.Net.BCrypt.HashPassword(registerDto.Password);

            // 3. Veritabanına DEĞİL, Business (İş) katmanına gönderiyoruz!
            await _appUserService.AddAsync(newUser);

            return Ok(201);  // 201 Created döndürüyoruz, mesaj yok.
        }

        // TÜM PERSONELLERİ LİSTELEME METODU (Sadece Admin görebilir)
        [HttpGet("GetAllPersonnel")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAllPersonnel()
        {
            var users = await _appUserService.GetAllAsync();

            // Güvenlik gereği şifreleri (hashleri) dışarı yollamıyoruz, AutoMapper ile temiz DTO'ya çeviriyoruz
            var usersDto = _mapper.Map<List<UserResponseDto>>(users);

            return Ok(usersDto);
        }

        [HttpGet("GetPersonnelById/{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetPersonnelById(int id)
        {
            var user = await _appUserService.GetByIdAsync(id);
            if (user == null) return NotFound();

            var userDto = _mapper.Map<UserResponseDto>(user);
            return Ok(userDto);
        }

        [HttpPut("UpdatePersonnel")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdatePersonnel(UserResponseDto userResponseDto)
        {
            var existingUser = await _appUserService.GetByIdAsync(userResponseDto.Id);
            if (existingUser == null) return NotFound();

            // Güvenlik Kalkanı: Admin rolünü yanlışlıkla "Personel" yapmayı engelleyebilirsin (İsteğe bağlı)
            if (existingUser.Role?.Trim().ToLower() == "admin" && userResponseDto.Role?.Trim().ToLower() != "admin")
            {
                return BadRequest(); // Sistemdeki Admin'in yetkisi düşürülemez.
            }

            // 2. Dışarıdan gelen DTO'daki bilgileri (Ad, Soyad, Rol) mevcut adama kopyalıyoruz
            existingUser.FirstName = userResponseDto.FirstName;
            existingUser.LastName = userResponseDto.LastName;
            existingUser.Role = userResponseDto.Role;
            // Şifreyi ve Kullanıcı adını BİLEREK değiştirtmiyoruz! (Güvenlik)

            _appUserService.Update(existingUser);
            return NoContent();  // 204 No Content: "İşlem başarılı ama sana dönecek bir mesajım yok" demek.
        }

        [HttpDelete("DeletePersonnel/{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeletePersonnel(int id)
        {
            var user = await _appUserService.GetByIdAsync(id);
            if(user == null) return NotFound();

            if (user.Role != null && user.Role.Trim().ToLower() == "admin")
            {
                // Sadece iş kurallarına aykırı bir durumda spesifik hata mesajı dönmek mantıklıdır
                return BadRequest("Sistemdeki bir admin hesabı silinemez!");
            }

            // ===================================================================================
            // GÜVENLİK AĞI (TRY-CATCH)
            // try: "Veritabanından silmeyi dene." Riskli işlemdir, veritabanı kurallarına takılabilir.
            // catch: "Eğer silerken veritabanı hata fırlatırs (örneğin adamın kargoları var diye silmeye izin vermezse)
            // programı çökertme, buraya düş ve durumu bizim kontrolümüzde yönet."
            // ===================================================================================
            try
            {
                _appUserService.Delete(user);
                return NoContent();  // 204 No Content: Başarılı bir silme işlemi sonrası standart yanıt
            }
            catch (Exception)
            {
                return StatusCode(500); // Sistem hatası, mesajı MVC tarafında biz uyduracağız.
            } 
        }
    }
}