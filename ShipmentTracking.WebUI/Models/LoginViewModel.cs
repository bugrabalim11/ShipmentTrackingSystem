using System.ComponentModel.DataAnnotations;

namespace ShipmentTracking.WebUI.Models
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Kullanıcı adı zorunludur.")]
        public string UserName { get; set; } = string.Empty;


        [Required(ErrorMessage = "Şifre zorunludur.")]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;
    }
}
