

using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace Diploma_project.ViewModels
{
    public class ResetPasswordViewModel
    {
        [HiddenInput(DisplayValue = false)]
        public string Id { get; set; }

        [Required(ErrorMessage = "Не указан email")]
        [RegularExpression(@"[A-Za-z0-9._%+-]+@[mail|gmail]*\.(ru|com)$", ErrorMessage = "Некорректный адрес. Только mail.ru или gmail.com")]
        [StringLength(100, ErrorMessage = "Длина строки должна быть до 100 символов")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Обязательное поле для ввода")]
        [DataType(DataType.Password)]
        [StringLength(50, MinimumLength = 8, ErrorMessage = "Длина пароль должна быть от 8 до 50 символов")]
        public string Password { get; set; }

        [Required(ErrorMessage = "Обязательное поле для ввода")]
        [System.ComponentModel.DataAnnotations.Compare("Password", ErrorMessage = "Пароли не совпадают")]
        [DataType(DataType.Password)]
        public string PasswordConfirm { get; set; }

        public string Code { get; set; }

    }
}
