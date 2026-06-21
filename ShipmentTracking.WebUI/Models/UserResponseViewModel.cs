namespace ShipmentTracking.WebUI.Models
{
    public class UserResponseViewModel
    {
        public int Id { get; set; }
        public string UserName { get; set; } = String.Empty;
        public string FirstName { get; set; } = String.Empty;
        public string LastName { get; set; } = String.Empty;
        public string Role { get; set; } = String.Empty;
    }
}
