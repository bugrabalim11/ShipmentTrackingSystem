using FluentValidation;
using ShipmentTracking.Entities.DTOs.Auth;
using ShipmentTracking.WebUI.Models;

namespace ShipmentTracking.WebUI.ValidationRules
{
    public class LoginViewModelValidator : AbstractValidator<LoginViewModel>
    {
        public LoginViewModelValidator()
        {
            RuleFor(x => x.UserName)
                .NotEmpty().WithMessage("Lütfen kullanıcı adını giriniz.");

            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("Lütfen şifrenizi giriniz.");
        }
    }
}
