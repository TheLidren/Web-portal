using System.ComponentModel.DataAnnotations;

namespace Diploma_project.ViewModels
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Обязательное поле для ввода")]
        [StringLength(80, ErrorMessage = "Длина строки должна быть до 100 символов", MinimumLength = 5)]
        [RegularExpression(@"[A-Za-z0-9._%+-]+(\@mail.ru|\@gmail.com)$", ErrorMessage = "Некорректный адрес (@mail.ru или @gmail.com)")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Обязательное поле для ввода")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}
