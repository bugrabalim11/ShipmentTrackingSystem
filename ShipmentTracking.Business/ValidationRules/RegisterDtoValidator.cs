
using FluentValidation;
using ShipmentTracking.Entities.DTOs.Auth;
using System;
using System.Collections.Generic;
using System.Text;

namespace ShipmentTracking.Business.ValidationRules
{
    public class RegisterDtoValidator : AbstractValidator<RegisterDto>
    {
        public RegisterDtoValidator()
        {
            RuleFor(x => x.FirstName)
                .NotEmpty().WithMessage("Ad alanı boş geçilemez.")
                .MinimumLength(2).WithMessage("Ad en az 2 karakter olmalıdır.")
                .MaximumLength(20).WithMessage("Ad en az 20 karaktr olmalıdır.");

            RuleFor(x => x.LastName)
                .NotEmpty().WithMessage("Soyad alanı boş geçilmez.")
                .MinimumLength(2).WithMessage("Soyad en az 2 karakter olmalıdır.")
                .MaximumLength(20).WithMessage("Soyad en fazla 20 karakter olmalıdır");

            RuleFor(x => x.UserName)
                .NotEmpty().WithMessage("Kullanıcı adı boş geçilemez.")
                .MinimumLength(5).WithMessage("Kullanıcı adı en az 5 karakter olmalıdır.")
                .MaximumLength(10).WithMessage("Kullanıcı adı en fazla 10 karakter olmalıdır.");

            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("Şifre alanı boş geçilemez.")
                .MinimumLength(6).WithMessage("Şifre en az 6 karakter olmalıdır.")
                .Matches("[A-Z]").WithMessage("Şifre en az bir büyük harf içermelidir.")
                .Matches("[a-z]").WithMessage("Şifre en az bir küçük harf içermelidir.")
                .Matches("[0-9]").WithMessage("Şifre en az bir rakam içermelidir.");

            RuleFor(x => x.Role)
                .NotEmpty().WithMessage("Rol belirtmek zorunludur.");
        }
    }
}
