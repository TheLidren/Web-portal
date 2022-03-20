using Diploma_project.App_Data;
using Diploma_project.Models;
using System.IO;
using System.Data.Entity;
using System.Web;
using System.Web.Mvc;
using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.AspNet.Identity.Owin;
using System.Threading.Tasks;

namespace Diploma_project.Controllers
{
    [Authorize]
    public class GaleryController : Controller
    {
        readonly PortalContext db = new();
        User user;
        readonly Regex trimmerspace = new(@"\s\s+");
        private ApplicationUserManager UserManager { get => HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>(); }

        [AllowAnonymous]
        public ActionResult ListPhoto() => View(db.FilesGaleries.Include(u => u.User).Where(u => u.Format == "image/jpeg" || u.Format == "image/png" || u.Format == "image/gif").Where(u => u.Status).ToList());

        [AllowAnonymous]
        public ActionResult ListVideo() => View(db.FilesGaleries.Include(u => u.User).Where(u => u.Format == "video/mp4").Where(u => u.Status).ToList());

        [HttpGet]
        public ActionResult AddPhoto() => View();

        [HttpGet]
        public ActionResult AddVideo() => View();

        //Publish picture on web-portal
        [HttpPost]
        public async Task<ActionResult> AddPhoto(FilesGalery filesGalery, HttpPostedFileBase[] uploadImage)
        {
            if (uploadImage[0] == null)
            {
                ModelState.AddModelError("uploadImage", $"Выберите файлы");
                return View(filesGalery);
            }
            if (ModelState.IsValid)
            {
                foreach (var file in uploadImage)
                {
                    byte[] imageData = null;
                    using (var binaryReader = new BinaryReader(file.InputStream))
                        imageData = binaryReader.ReadBytes(file.ContentLength);
                    filesGalery.Tittle = trimmerspace.Replace(filesGalery.Tittle, " ").Trim();
                    filesGalery.FileString = imageData;
                    filesGalery.FileName = file.FileName;
                    filesGalery.Format = file.ContentType;
                    filesGalery.Status = true;
                    user = await UserManager.FindByEmailAsync(User.Identity.Name);
                    filesGalery.UserId = user.Id;
                    db.FilesGaleries.Add(filesGalery);
                    db.SaveChanges();
                }
                return RedirectToAction("ListPhoto");
            }
            return View(filesGalery);
        }

        [HttpGet]
        public ActionResult EditPhoto(int? id)
        {
            FilesGalery filesGalery = db.FilesGaleries.Find(id);
            if (filesGalery == null)
                return RedirectToAction("Error", "Home");
            return View(filesGalery);
        }

        [HttpGet]
        public ActionResult EditVideo(int? id)
        {
            FilesGalery filesGalery = db.FilesGaleries.Find(id);
            if (filesGalery == null)
                return RedirectToAction("Error", "Home");
            return View(filesGalery);
        }

        //Edit selected picture on web-portal
        [HttpPost]
        public async Task<ActionResult> EditPhoto(FilesGalery filesGalery, HttpPostedFileBase uploadImage)
        {
            if (ModelState.IsValid)
            {
                if (uploadImage != null)
                {
                    byte[] imageData = null;
                    using (var binaryReader = new BinaryReader(uploadImage.InputStream))
                        imageData = binaryReader.ReadBytes(uploadImage.ContentLength);
                    filesGalery.FileString = imageData;
                    filesGalery.FileName = uploadImage.FileName;
                    filesGalery.Format = uploadImage.ContentType;
                    user = await UserManager.FindByEmailAsync(User.Identity.Name);
                    filesGalery.UserId = user.Id;
                }
                db.Entry(filesGalery).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("ListPhoto");
            }
            return View(filesGalery);
        }

        //Edit selected video in table "FilesGalery"
        [HttpPost]
        public async Task<ActionResult> EditVideo(FilesGalery filesGalery)
        {
            if (ModelState.IsValid)
            {
                int tittle = filesGalery.Tittle.LastIndexOf("/");
                filesGalery.FileName = filesGalery.Tittle.Substring(tittle + 1).Replace(" ", "");
                user = await UserManager.FindByEmailAsync(User.Identity.Name);
                filesGalery.UserId = user.Id;
                db.Entry(filesGalery).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("ListVideo");
            }
            return View(filesGalery);
        }

        //Add selected video in table "FilesGalery"
        [HttpPost]
        public async Task<ActionResult> AddVideo(FilesGalery filesGalery)
        {
            if (ModelState.IsValid)
            {
                int tittle = filesGalery.Tittle.LastIndexOf("/");
                filesGalery.FileName = filesGalery.Tittle.Substring(tittle+1).Replace(" ", "");
                filesGalery.Format = "video/mp4";
                filesGalery.Status = true;
                user = await UserManager.FindByEmailAsync(User.Identity.Name);
                filesGalery.UserId = user.Id;
                db.FilesGaleries.Add(filesGalery);
                db.SaveChanges();
                return RedirectToAction("ListVideo");
            }
            return View(filesGalery);
        }

        //Delete selected media on web-portal
        [HttpGet, Authorize(Roles = "programmer")]
        public ActionResult DeleteFilesGalery(int? id)
        {
            FilesGalery files = db.FilesGaleries.Find(id);
            if (files == null)
                return RedirectToAction("Error", "Home");
            files.Status = false;
            db.SaveChanges();
            if (files.Format == "video/mp4")
                return RedirectToAction("ListVideo");
            return RedirectToAction("ListPhoto");
        }
    }
}