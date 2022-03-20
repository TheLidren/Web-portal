using Diploma_project.Models;
using System.ComponentModel.DataAnnotations;

namespace Diploma_project.ViewModels
{
    public class RegisterViewModel
    {
        [Required(ErrorMessage = "Не указано имя")]
        [RegularExpression(@"^([А-ЯЁ]{1}[а-яё]{1,50})$", ErrorMessage = "Текст должен содержать только латинские буквы и нижний регистр")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "Длина строки должна быть от 3 до 50 символов")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Не указана фамилия")]
        [RegularExpression(@"^([А-ЯЁ]{1}[а-яё]{1,50})$", ErrorMessage = "Текст должен содержать только латинские буквы и нижний регистр")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "Длина строки должна быть от 3 до 50 символов")]
        public string Surname { get; set; }

        [Required(ErrorMessage = "Не указано отчество")]
        [RegularExpression(@"^([А-ЯЁ]{1}[а-яё]{1,50})$", ErrorMessage = "Текст должен содержать только латинские буквы и нижний регистр")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "Длина строки должна быть от 3 до 50 символов")]
        public string Patronymic { get; set; }

        [Required(ErrorMessage = "Обязательное поле для ввода")]
        [StringLength(80, ErrorMessage = "Длина строки должна быть до 100 символов", MinimumLength = 5)]
        [RegularExpression(@"[A-Za-z0-9._%+-]+(\@mail.ru|\@gmail.com)$", ErrorMessage = "Некорректный адрес (@mail.ru или @gmail.com)")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Выберите должность")]
        public int? PositionId { get; set; }

        public Position Position { get; set; }

        [Required(ErrorMessage = "Обязательное поле для ввода")]
        [DataType(DataType.Password)]
        [StringLength(50, MinimumLength = 8, ErrorMessage = "Длина пароль должна быть от 8 до 50 символов")]
        public string Password { get; set; }

        [Required(ErrorMessage = "Обязательное поле для ввода")]
        [Compare("Password", ErrorMessage = "Пароли не совпадают")]
        [DataType(DataType.Password)]
        public string PasswordConfirm { get; set; }
    }
}
