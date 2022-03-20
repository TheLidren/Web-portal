using System;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace Diploma_project.Models
{
    public class FilesDocumentsFavorites
    {
        [HiddenInput(DisplayValue = false)]
        public int Id { get; set; }
        public int? FilesDocumentsId { get; set; }
        public FilesDocuments FilesDocuments { get; set; }
        public string UserId { get; set; }
        public User User { get; set; }

    }
}
