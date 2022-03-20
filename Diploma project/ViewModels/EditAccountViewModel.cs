

using Diploma_project.Models;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace Diploma_project.ViewModels
{
    public class EditAccountViewModel
    {
        [HiddenInput(DisplayValue = false)]
        public string Id { get; set; }

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

        [Required(ErrorMessage = "Выберите должность")]
        public int? PositionId { get; set; }

        public Position Position { get; set; }
    }
}
