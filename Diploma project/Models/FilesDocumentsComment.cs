using System;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace Diploma_project.Models
{
    public class FilesDocumentsComment
    {
        [HiddenInput(DisplayValue = false)]
        public int Id { get; set; }

        public string UserName { get; set; }

        public DateTime When { get; set; }

        [Required(ErrorMessage = "Обязательное поле для ввода")]
        public string Text { get; set; }

        public int? FilesDocumentsId { get; set; }

        public FilesDocuments FilesDocuments { get; set; }

    }
}
