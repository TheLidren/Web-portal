using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace Diploma_project.Models
{
    public class News
    {
        [HiddenInput(DisplayValue = false)]
        public int Id { get; set; }

        [Required(ErrorMessage = "Обязательное поле для ввода")]
        [StringLength(100, ErrorMessage = "Введите корректно размерность", MinimumLength = 5)]
        [RegularExpression(@"^([а-яА-ЯёЁa-zA-Z0-9\s-_.№""():!]{1,100})$", ErrorMessage = "Введите корректно заголовок")]
        public string Tittle { get; set; }

        [Required(ErrorMessage = "Обязательное поле для ввода")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:yyyy-MM-dd}")]
        public DateTime Date { get; set; }

        [Required(ErrorMessage = "Обязательное поле для ввода")]
        public string LongDesc { get; set; }

        public string UserId { get; set; }
        public User User { get; set; }

        public virtual ICollection<NewsImage> NewsImages { get; set; }

    }
}
