using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ShipmentTracking.DataAccess.Concrete.EntityFramework;
using ShipmentTracking.Entities.DTOs.Auth;

namespace ShipmentTracking.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly AppDbContext _appDbContext;
        private readonly IMapper _mapper;  // AutoMapper eklendi

        // Veritabanına doğrudan bağlanmak için DbContext'i çağırıyoruz
        public AuthController(AppDbContext appDbContext, IMapper mapper)
        {
            _appDbContext = appDbContext;
            _mapper = mapper;
        }

        [HttpPost("Login")]
        public IActionResult Login(LoginDto loginDto)
        {
            // Gelen kullanıcı adı ve şifreyi veritabanındaki AppUsers tablosunda arıyoruz
            var user = _appDbContext.AppUsers.FirstOrDefault(x =>
            x.UserName == loginDto.UserName && x.Password == loginDto.Password);

            if (user == null)
            {
                // Eğer eşleşme yoksa 401 Unauthorized (Yetkisiz/Hatalı) dönüyoruz
                return Unauthorized();
            }

            // İŞTE SİHİR BURADA: Tek satırda dönüşüm!
            var responseDto = _mapper.Map<UserResponseDto>(user);

            return Ok(responseDto);
        }
    }
}
