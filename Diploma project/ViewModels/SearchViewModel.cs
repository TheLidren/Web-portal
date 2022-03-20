using Diploma_project.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Diploma_project.ViewModels
{
    public class SearchViewModel
    {
        public IEnumerable<News> News { get; set; }
        public IEnumerable<FilesDocuments> FilesDocuments { get; set; }
    }
}
