using System.Web.Mvc;

namespace Diploma_project.Models
{
    public class NewsImage
    {
        [HiddenInput(DisplayValue = false)]
        public int Id { get; set; }

        public int? NewsId { get; set; }

        public News News { get; set; }

        public byte[] ImageString { get; set; }
    }
}
