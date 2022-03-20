using Diploma_project.Models;
using System.Collections.Generic;

namespace Diploma_project.ViewModels
{
    public class DocumentsViewModel
    {
        public FilesDocuments FilesDocuments { get; set; }

        public IEnumerable<FilesDocumentsComment> FilesDocumentsComments { get; set; }

        public FilesDocumentsFavorites FilesDocumentsFavorites { get; set; }

    }
}
