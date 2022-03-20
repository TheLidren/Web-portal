using Diploma_project.Models;
using System.Collections.Generic;

namespace Diploma_project.ViewModels
{
    public class NewsViewModel
    {
        public News News { get; set; }

        public IEnumerable<NewsImage> Images { get; set; }

    }
}
