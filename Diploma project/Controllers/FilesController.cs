using Diploma_project.App_Data;
using Diploma_project.Models;
using Diploma_project.ViewModels;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Office.Interop.Word;
using PagedList;
using System;
using System.Data.Entity;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Mime;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace Diploma_project.Controllers
{
    public class FilesController : Controller
    {
        readonly PortalContext db = new();
        readonly Regex trimmerspace = new(@"\s\s+");
        readonly int pageSize = 5;
        User user;
        private ApplicationUserManager UserManager { get => HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>(); }

        //Convert Words document to HTML
        public string ReceiveWordHtml(FilesDocuments files)
        {
            object documentFormat = 8;
            object htmlFilePath = Server.MapPath("~/Files/Temp/") + files.FileName.Substring(0, files.FileName.LastIndexOf(".")) + ".htm";
            object fileSavePath = files.FilePath;
            if (!System.IO.File.Exists(htmlFilePath.ToString()))
            {
                _Application applicationclass = new Application();
                applicationclass.Documents.Open(ref fileSavePath);
                applicationclass.Visible = false;
                Document document = applicationclass.ActiveDocument;
                document.SaveAs(ref htmlFilePath, ref documentFormat);
                document.Close();
            }
            string wordHTML = null;
            using (FileStream fstream = System.IO.File.OpenRead(htmlFilePath.ToString()))
            {
                byte[] buffer = new byte[fstream.Length];
                fstream.Read(buffer, 0, buffer.Length);
                wordHTML = Encoding.Default.GetString(buffer);
                fstream.Close();
            }
            foreach (Match match in Regex.Matches(wordHTML, "<v:imagedata.+?src=[\"'](.+?)[\"'].*?>", RegexOptions.IgnoreCase))
                wordHTML = Regex.Replace(wordHTML, match.Groups[1].Value, "Files/Temp/" + match.Groups[1].Value);
            return wordHTML;
        }

        [HttpGet]
        public ActionResult ListDocuments(int? page)
        {
            int pageNumber = (page ?? 1);
            return View(db.FilesDocuments.Include(u => u.User).OrderByDescending(u => u.DatePublish).Where(u => u.Status).ToPagedList(pageNumber, pageSize));
        }

        [HttpGet, Authorize]
        public async Task<ActionResult> ListDocumentsFavorites()
        {
            user = await UserManager.FindByEmailAsync(User.Identity.Name);
            return View(db.filesDocumentsFavorites.Include(u => u.FilesDocuments).Include(u => u.FilesDocuments.User).OrderByDescending(u => u.FilesDocuments.DatePublish).Where(u => u.FilesDocuments.Status && u.UserId == user.Id).ToList());
        }

        [HttpGet]
        public async Task<ActionResult> ShowDocument(int? id)
        {
            FilesDocuments files = await db.FilesDocuments.FindAsync(id);
            if (files == null)
                return RedirectToAction("Error", "Home");
            FilesDocumentsFavorites filesDocumentsFavorites = new();
            if(User.Identity.Name != "")
            {
                user = await UserManager.FindByEmailAsync(User.Identity.Name);
                filesDocumentsFavorites = db.filesDocumentsFavorites.Where(u => u.FilesDocumentsId == files.Id && u.UserId == user.Id).FirstOrDefault();
            }
            DocumentsViewModel viewModel = new()
            {
                FilesDocuments = files,
                FilesDocumentsComments = db.FilesDocumentsComments.Where(u => u.FilesDocumentsId == files.Id).OrderByDescending(u => u.When).ToList(),
                FilesDocumentsFavorites = filesDocumentsFavorites
            };
            string wordHTML = ReceiveWordHtml(files);
            ViewBag.WordHtml = wordHTML.Substring(wordHTML.IndexOf("<div class=WordSection1>"));
            return View(viewModel);
        }


        [HttpGet, Authorize]
        public ActionResult AddDocument() => View();

        //Publish document on web-portal
        [HttpPost, Authorize]
        public async Task<ActionResult> AddDocument(FilesDocuments files, HttpPostedFileBase uploadDocx)
        {
            if (files.DatePublish >= DateTime.Now.AddMonths(2) || files.DatePublish <= DateTime.Now.AddMonths(-1))
            {
                ModelState.AddModelError("DatePublish", $"Укажите дату от " +
                    $"{DateTime.Now.AddMonths(-1).ToShortDateString()} до {DateTime.Now.AddMonths(2).ToShortDateString()}");
                return View(files);
            }
            if (ModelState.IsValid && uploadDocx != null)
            {
                string path = Server.MapPath("~/Files/" + uploadDocx.FileName);
                files.Tittle = trimmerspace.Replace(files.Tittle, " ").Trim();
                files.FilePath = path;
                files.FileName = uploadDocx.FileName;
                user = await UserManager.FindByEmailAsync(User.Identity.Name);
                files.UserId = user.Id;
                files.Status = true;
                FilesDocuments filesDocuments = db.FilesDocuments.Where(u => u.FilePath == files.FilePath)
                    .Where(u => u.Status).FirstOrDefault();
                if (filesDocuments != null)
                {
                    ModelState.AddModelError("uploadDocx", $"Документ с таким же названием уже опубликован");
                    return View(files);
                }
                uploadDocx.SaveAs(path);
                db.FilesDocuments.Add(files);
                db.SaveChanges();
                return RedirectToAction("ListDocuments");
            }
            ModelState.AddModelError("uploadDocx", $"Выберите файл");
            return View(files);
        }

        [HttpGet, Authorize]
        public ActionResult EditDocument(int? id)
        {
            FilesDocuments files = db.FilesDocuments.Find(id);
            if (files == null)
                return RedirectToAction("Error", "Home");
            return View(files);
        }

        //Edit selected document 
        [HttpPost, Authorize]
        public async Task<ActionResult> EditDocument(FilesDocuments files, HttpPostedFileBase uploadDocx)
        {
            if (ModelState.IsValid)
            {
                string pathOldDocument = files.FilePath;
                if (uploadDocx != null)
                {
                    string path = Server.MapPath("~/Files/" + uploadDocx.FileName);
                    FilesDocuments filesDocuments = db.FilesDocuments.Where(u => u.FilePath == path).Where(u => u.Status).FirstOrDefault();
                    if (filesDocuments != null)
                    {
                        ModelState.AddModelError("uploadDocx", $"Документ с таким же названием уже опубликован");
                        return View(files);
                    }
                    System.IO.File.Delete(pathOldDocument);
                    uploadDocx.SaveAs(path);
                    files.FilePath = path;
                    files.FileName = uploadDocx.FileName;
                }
                files.Tittle = trimmerspace.Replace(files.Tittle, " ").Trim();
                user = await UserManager.FindByEmailAsync(User.Identity.Name);
                files.UserId = user.Id;
                db.Entry(files).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("ListDocuments");
            }
            return View(files);
        }

        //Delete document from server folder
        [HttpGet, Authorize(Roles = "programmer")]
        public ActionResult DeleteDocument(int? id)
        {
            FilesDocuments files = db.FilesDocuments.Find(id);
            if (files == null)
                return RedirectToAction("Error", "Home");
            string path = Server.MapPath("~/Files/Deletefiles/" + files.FileName);
            System.IO.File.Copy(files.FilePath, path, true);
            System.IO.File.Delete(files.FilePath);
            files.FilePath = path;
            files.Status = false;
            db.Entry(files).State = EntityState.Modified;
            db.SaveChanges();
            string randomName = files.FileName.Substring(0, files.FileName.LastIndexOf("."));
            object htmlFilePath = Server.MapPath("~/Files/Temp/") + randomName + ".htm";
            if (System.IO.File.Exists(htmlFilePath.ToString()))
            {
                System.IO.File.Delete(htmlFilePath.ToString());
                Directory.Delete(Server.MapPath("~/Files/Temp/") + randomName + ".files", true);
            }
            return RedirectToAction("ListDocuments");
        }

        //Download document on computer
        [HttpGet]
        public ActionResult DownloadDocument(int? id)
        {
            FilesDocuments files = db.FilesDocuments.Find(id);
            if (files == null)
                return RedirectToAction("Error", "Home");
            return File(System.IO.File.ReadAllBytes(files.FilePath), MediaTypeNames.Application.Octet, files.FileName);
        }

        //Publish comment for selected document
        [HttpGet, Authorize]
        public ActionResult SendComment(FilesDocumentsComment filesDocumentsComment)
        {
            if (filesDocumentsComment.Text != null)
            {
                filesDocumentsComment.UserName = User.Identity.Name;
                filesDocumentsComment.When = DateTime.Now;
                filesDocumentsComment.Text = trimmerspace.Replace(filesDocumentsComment.Text, " ").Trim();
                db.FilesDocumentsComments.Add(filesDocumentsComment);
                db.SaveChanges();
                return RedirectToAction("ShowDocument", new { id = filesDocumentsComment.FilesDocumentsId });
            }
            return RedirectToAction("ShowDocument", new { id = filesDocumentsComment.FilesDocumentsId });
        }

        [HttpGet, Authorize]
        public async Task<ActionResult> SaveFavorites(int? id)
        {
            FilesDocuments files = await db.FilesDocuments.FindAsync(id);
            if (files == null)
                return RedirectToAction("Error", "Home");
            user = await UserManager.FindByEmailAsync(User.Identity.Name);
            FilesDocumentsFavorites filesDocumentsFavorites = db.filesDocumentsFavorites.Where(u => u.FilesDocumentsId == files.Id && u.UserId == user.Id).FirstOrDefault();
            if (filesDocumentsFavorites == null)
            {
                filesDocumentsFavorites = new() { FilesDocumentsId = id, UserId = user.Id };
                db.filesDocumentsFavorites.Add(filesDocumentsFavorites);
            }
            else
                db.filesDocumentsFavorites.Remove(filesDocumentsFavorites);
            await db.SaveChangesAsync();
            return RedirectToAction("ShowDocument", new { id = files.Id });
        }

        //open version for print
        [HttpGet]
        public ActionResult PrintDocument(int? id)
        {
            FilesDocuments files = db.FilesDocuments.Find(id);
            if (files == null)
                return RedirectToAction("Error", "Home");
            string wordHTML = ReceiveWordHtml(files);
            ViewBag.WordHtml = wordHTML.Substring(wordHTML.IndexOf("<div class=WordSection1>"));
            return View();
        }

        [HttpGet, Authorize(Roles = "programmer")]
        public ActionResult DeleteComment(int id)
        {
            FilesDocumentsComment files = db.FilesDocumentsComments.Find(id);
            if (files == null)
                return RedirectToAction("Error", "Home");
            int? filesDocumentsId = files.FilesDocumentsId;
            db.FilesDocumentsComments.Remove(files);
            db.SaveChanges();
            return RedirectToAction("ShowDocument", new { id = filesDocumentsId });
        }
    }
}