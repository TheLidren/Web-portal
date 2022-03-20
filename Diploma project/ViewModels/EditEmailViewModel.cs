using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace Diploma_project.ViewModels
{
    public class EditEmailViewModel
    {
        [HiddenInput(DisplayValue = false)]
        public string Id { get; set; }

        [Required(ErrorMessage = "Обязательное поле для ввода")]
        [StringLength(80, ErrorMessage = "Длина строки должна быть до 100 символов", MinimumLength = 5)]
        [RegularExpression(@"[A-Za-z0-9._%+-]+(\@mail.ru|\@gmail.com)$", ErrorMessage = "Некорректный адрес (@mail.ru или @gmail.com)")]
        public string Email { get; set; }
    }
}
