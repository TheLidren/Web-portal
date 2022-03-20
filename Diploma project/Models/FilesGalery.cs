using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace Diploma_project.Models
{
    public class FilesGalery
    {
        [HiddenInput(DisplayValue = false)]
        public int Id { get; set; }

        [Required(ErrorMessage = "Обязательное поле для ввода")]
        [StringLength(100, ErrorMessage = "Введите корректно размерность", MinimumLength = 2)]
        [RegularExpression(@"^([а-яА-ЯёЁa-zA-Z0-9\s\,-_.""\\/:]{1,100})$", ErrorMessage = "Введите корректно название")]
        public string Tittle { get; set; }

        public string FileName { get; set; }

        public string Format { get; set; }

        public byte[] FileString { get; set; }
        public string UserId { get; set; }
        public User User { get; set; }

        public bool Status { get; set; }
    }
}
