using Diploma_project.App_Data;
using Diploma_project.ViewModels;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;

namespace Diploma_project.Controllers
{
    public class HomeController : Controller
    {
        readonly PortalContext db = new();

        [AllowAnonymous, HttpGet]
        public ActionResult Index() => View(db.News.Include(u => u.User).OrderByDescending(u => u.Date).ToList());

        [HttpGet, AllowAnonymous]
        public ActionResult Error() => View();

        [HttpGet, Authorize(Roles = "programmer")]
        public ActionResult ListUser()
        {
            UserPositionViewModel viewModel = new()
            {
                Users = db.Users.Include(u => u.Position).Where(u => u.Status).ToList(),
                Positions = db.Positions.ToList()
            };
            return View(viewModel);
        }

        [HttpPost, AllowAnonymous]
        public ActionResult SearchList(string searchInfo)
        {
            SearchViewModel viewModel = new()
            {
                News = db.News.Include(u => u.User).OrderByDescending(u => u.Date).Where(u => u.Tittle.Contains(searchInfo) || u.User.UserName.Contains(searchInfo)).ToList(),
                FilesDocuments = db.FilesDocuments.Include(u => u.User).OrderByDescending(u => u.DatePublish).Where(u => u.Status).Where(u => u.Tittle.Contains(searchInfo) || u.User.UserName.Contains(searchInfo)).ToList()
            };
            ViewBag.SearchInfo = searchInfo;
            return View(viewModel);
        }
    }
}