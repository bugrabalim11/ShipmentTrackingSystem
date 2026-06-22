using System.ComponentModel.DataAnnotations;

namespace ShipmentTracking.WebUI.Models
{
    public class LoginViewModel
    {
        public string UserName { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }
}
