using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace Diploma_project.Models
{
    public class Position
    {
        [HiddenInput(DisplayValue = false)]
        public int Id { get; set; }

        [Required(ErrorMessage = "Обязательное поле для ввода")]
        [StringLength(50, ErrorMessage = "Введите корректно размерность", MinimumLength = 2)]
        [RegularExpression(@"^([А-ЯЁ]{1}[а-яё\s-]{1,50})$", ErrorMessage = "Текст должен содержать только латинские буквы и дефис")]
        public string Tittle { get; set; }

        public bool Status { get; set; }

        public virtual ICollection<ContactInformation> Contacts { get; set; }
        public virtual ICollection<User> Users { get; set; }
    }
}
