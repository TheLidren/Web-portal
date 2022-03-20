using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace Diploma_project.Models
{
    public class ContactInformation
    {
        [HiddenInput(DisplayValue = false)]
        public int Id { get; set; }

        [Required(ErrorMessage = "Обязательное поле для ввода")]
        [StringLength(50, ErrorMessage = "Введите корректно размерность", MinimumLength = 2)]
        [RegularExpression(@"^([А-ЯЁ]{1}[а-яё]{1,50})$", ErrorMessage = "Текст содержит только латинские буквы с соблюдением регистра")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Обязательное поле для ввода")]
        [StringLength(50, ErrorMessage = "Введите корректно размерность", MinimumLength = 2)]
        [RegularExpression(@"^([А-ЯЁ]{1}[а-яё\-]{1,50})$", ErrorMessage = "Текст содержит только латинские буквы с соблюдением регистра")]
        public string Surname { get; set; }

        [Required(ErrorMessage = "Обязательное поле для ввода")]
        [StringLength(50, ErrorMessage = "Введите корректно размерность", MinimumLength = 2)]
        [RegularExpression(@"^([А-ЯЁ]{1}[а-яё]{1,50})$", ErrorMessage = "Текст содержит только латинские буквы с соблюдением регистра")]
        public string Patronymic { get; set; }

        [Required(ErrorMessage = "Выберите должность")]
        public int? PositionId { get; set; }

        public Position Position { get; set; }

        [Required(ErrorMessage = "Обязательное поле для ввода")]
        [StringLength(15, ErrorMessage = "Введите корректно размерность", MinimumLength = 10)]
        [RegularExpression(@"^(\+375|80)(29|25|44|33)(\d{7})$", ErrorMessage = "Некорректно введён номер телефона")]
        public string PhoneNumber { get; set; }
    }
}
