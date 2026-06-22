using FluentValidation;
using ShipmentTracking.Entities.DTOs.Auth;
using System;
using System.Collections.Generic;
using System.Text;

namespace ShipmentTracking.Business.ValidationRules
{
    public class LoginDtoValidator : AbstractValidator<LoginDto>
    {
        public LoginDtoValidator()
        {
            RuleFor(x => x.UserName)
                .NotEmpty().WithMessage("Lütfen kullanıcı adınızı giriniz.");

            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("Lütfen şifrenizi giriniz.");
        }
    }
}
