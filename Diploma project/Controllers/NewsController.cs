using Diploma_project.App_Data;
using Diploma_project.Models;
using System;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Diploma_project.ViewModels;
using Microsoft.AspNet.Identity.Owin;
using System.Threading.Tasks;
using System.Data.Entity;
using System.Text.RegularExpressions;
using System.Collections.Generic;

namespace Diploma_project.Controllers
{
    [Authorize(Roles = "secretary, programmer, direktor")]
    public class NewsController : Controller
    {

        readonly PortalContext db = new();
        User user;
        readonly Regex trimmerspace = new(@"\s\s+");
        private ApplicationUserManager UserManager { get => HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>(); }

        readonly string replace = "</p><p class='pb-2' style='font-size: 18px; font-family: 'Noto Sans', sans-serif; letter-spacing: 2px; text-indent: 2em; '>";

        [HttpGet, AllowAnonymous]
        public ActionResult ListNews() => View(db.News.Include(u => u.User).OrderByDescending(u => u.Date).ToList());

        [HttpGet]
        public ActionResult AddNews() => View();

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<ActionResult> AddNews(News news, HttpPostedFileBase[] uploadImage)
        {
            if (news.Date >= DateTime.Now.AddMonths(2) || news.Date <= DateTime.Now.AddMonths(-2))
            {
                ModelState.AddModelError("DatePublish", $"Укажите дату от {DateTime.Now.AddMonths(-2).ToShortDateString()} до {DateTime.Now.AddMonths(2).ToShortDateString()}");
                return View(news);
            }
            if (ModelState.IsValid && uploadImage[0] != null)
            {
                news.Tittle = trimmerspace.Replace(news.Tittle, " ").Trim();
                if(Regex.IsMatch(news.LongDesc, "\r\n"))
                    news.LongDesc = news.LongDesc.Replace("\r\n", replace);
                else
                    news.LongDesc += replace;
                user = await UserManager.FindByEmailAsync(User.Identity.Name);
                news.UserId = user.Id;
                db.News.Add(news);
                db.SaveChanges();
                foreach (var file in uploadImage)
                {
                    byte[] imageData = null;
                    using (var binaryReader = new BinaryReader(file.InputStream))
                        imageData = binaryReader.ReadBytes(file.ContentLength);
                    NewsImage image = new() { NewsId = news.Id, ImageString = imageData };
                    db.NewsImages.Add(image);
                    db.SaveChanges();
                }
                return RedirectToAction("ListNews");
            }
            ModelState.AddModelError("uploadImage", $"Выберите файл");
            return View(news);
        }

        [HttpGet, AllowAnonymous]
        public ActionResult DetailsNews(int? id)
        {
            NewsViewModel viewModel = new()
            {
                News = db.News.Where(u => u.Id == id).FirstOrDefault(),
                Images = db.NewsImages.Where(u => u.NewsId == id).ToList()
            };

            return View(viewModel);
        }

        [HttpGet]
        public ActionResult EditNews(int? id)
        {
            News news = db.News.Find(id);
            if (news == null)
                return RedirectToAction("Error", "Home");
            news.LongDesc = news.LongDesc.Replace(replace, "\r\n");
            return View(news);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<ActionResult> EditNews(News news, HttpPostedFileBase[] uploadImage)
        {
            if (ModelState.IsValid)
            {
                if (uploadImage[0] != null)
                {
                    List<NewsImage> newsImages = db.NewsImages.Where(u => u.NewsId == news.Id).ToList();
                    db.NewsImages.RemoveRange(newsImages);
                    await db.SaveChangesAsync();
                    foreach (var file in uploadImage)
                    {
                        byte[] imageData = null;
                        using (var binaryReader = new BinaryReader(file.InputStream))
                            imageData = binaryReader.ReadBytes(file.ContentLength);
                        NewsImage image = new() { NewsId = news.Id, ImageString = imageData };
                        db.NewsImages.Add(image);
                        await db.SaveChangesAsync();
                    }
                }
                news.Tittle = trimmerspace.Replace(news.Tittle, " ").Trim();
                if (Regex.IsMatch(news.LongDesc, "\r\n"))
                    news.LongDesc = news.LongDesc.Replace("\r\n", replace);
                else
                    news.LongDesc += replace;
                user = await UserManager.FindByEmailAsync(User.Identity.Name);
                news.UserId = user.Id;
                db.Entry(news).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("ListNews");
            }
            return View(news);
        }

        [HttpGet, Authorize(Roles = "programmer")]
        public ActionResult DeleteNews(int? id)
        {
            News news = db.News.Find(id);
            if (news == null)
                return RedirectToAction("Error", "Home");
            foreach (var item in db.NewsImages.Where(u => u.NewsId == id).ToList())
            {
                db.NewsImages.Remove(item);
                db.SaveChanges();
            }
            db.News.Remove(news);
            db.SaveChanges();
            return RedirectToAction("ListNews");
        }
    }
}
