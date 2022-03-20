using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace Diploma_project.Models
{
    public class Services
    {
        [HiddenInput(DisplayValue = false)]
        public int Id { get; set; }

        [Required(ErrorMessage = "Обязательное поле для ввода")]
        [RegularExpression(@"^([А-ЯЁ]{1}[а-яё\s-,:]{1,200})$", ErrorMessage = "Проверьте корректность ввода раздела услуг")]
        public string Tittle { get; set; }

        [Required(ErrorMessage = "Обязательное поле для ввода")]
        [RegularExpression(@"^[а-яА-ЯёЁ0-9a-z\rn\s\-',:;.<>=\\/]+$", ErrorMessage = "Проверьте корректность ввода")]
        public string ListServices { get; set; }
        public string UserId { get; set; }
        public User User { get; set; }

    }
}
