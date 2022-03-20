using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace Diploma_project.Models
{
    public class FilesDocuments
    {
        [HiddenInput(DisplayValue = false)]
        public int Id { get; set; }

        [Required(ErrorMessage = "Обязательное поле для ввода")]
        [StringLength(100, ErrorMessage = "Введите корректно размерность", MinimumLength = 5)]
        [RegularExpression(@"^([а-яА-ЯёЁa-zA-Z0-9\s-_.№""():]{1,100})$", ErrorMessage = "Введите корректно заголовок")]
        public string Tittle { get; set; }

        [Required(ErrorMessage = "Обязательное поле для ввода")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:yyyy-MM-dd}")]
        public DateTime DatePublish { get; set; }

        public string FileName { get; set; }

        public string FilePath { get; set; }
        public string UserId { get; set; }
        public User User { get; set; }
        public bool Status { get; set; }

        public virtual ICollection<FilesDocumentsComment> FilesDocumentsComments { get; set; }

    }
}
