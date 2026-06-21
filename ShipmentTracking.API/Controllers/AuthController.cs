using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using ShipmentTracking.Business.Abstract;
using ShipmentTracking.Entities.Concrete;
using ShipmentTracking.Entities.DTOs.Auth;
using System.Linq;
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

        public AuthController(IAppUserService appUserService, IMapper mapper)
        {
            _appUserService = appUserService;
            _mapper = mapper;
        }

        [HttpPost("Login")]
        public async Task<IActionResult> Login(LoginDto loginDto)
        {
            // 1. Generic servisimizden tüm kullanıcıları çekiyoruz
            var users = await _appUserService.GetAllAsync();

            // 2. Gelen kullanıcı adı ve şifreyi bu listede arıyoruz
            var user = users.FirstOrDefault(x => x.UserName == loginDto.UserName && x.Password == loginDto.Password);

            if (user == null)
            {
                return Unauthorized();
            }

            // 3. Bulunan kullanıcıyı DTO'ya çevirip yolluyoruz
            var responseDto = _mapper.Map<UserResponseDto>(user);
            return Ok(responseDto);
        }

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

            // 3. Veritabanına DEĞİL, Business (İş) katmanına gönderiyoruz!
            await _appUserService.AddAsync(newUser);

            return Ok();
        }
    }
}