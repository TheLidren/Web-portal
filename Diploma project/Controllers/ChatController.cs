using Diploma_project.App_Data;
using Diploma_project.Models;
using Diploma_project.ViewModels;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net.Mime;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;

namespace Diploma_project.Controllers
{
    [Authorize]
    public class ChatController : Controller
    {
        readonly PortalContext db = new();
        User user;
        List<Message> messageList;
        readonly Regex trimmerspace = new(@"\s\s+");
        private ApplicationUserManager UserManager { get => HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>(); }
        private ApplicationRoleManager RoleManager { get => HttpContext.GetOwinContext().GetUserManager<ApplicationRoleManager>(); }
        const int pageSize = 10;

        //Show users chat
        [HttpGet]
        public ActionResult ChatAccount(string userId, int? id)
        {
            int page = id ?? 1;
            User myUser = null;
            myUser = UserManager.FindByEmail(User.Identity.Name);
            if (userId != null)
            {
                user = db.Users.Find(userId);
                if (user.Id == myUser.Id)
                {
                    var itemsToSkip = page * pageSize;
                    messageList = db.Messages.Where(u => u.UserName == myUser.UserName && u.UserId == myUser.Id).Where(u => u.PersonalMessage).OrderBy(m => m.When).Take(itemsToSkip).ToList();
                    ViewBag.Count = db.Messages.Where(u => u.UserName == myUser.UserName && u.UserId == myUser.Id).Where(u => u.PersonalMessage).Count();
                    foreach (var item in messageList)
                        if (item.UserId == myUser.Id)
                        {
                            item.Read = true;
                            db.Entry(item).State = EntityState.Modified;
                            db.SaveChanges();
                        }
                }
                else
                {
                    var itemsToSkip = page * pageSize;
                    messageList = db.Messages.Where(u => u.UserName == myUser.UserName && u.UserId == user.Id || u.UserName == user.UserName && u.UserId == myUser.Id).Where(u => !u.PersonalMessage).OrderBy(m => m.When).Take(pageSize).ToList();
                    ViewBag.Count = db.Messages.Where(u => u.UserName == myUser.UserName && u.UserId == user.Id || u.UserName == user.UserName && u.UserId == myUser.Id).Where(u => !u.PersonalMessage).Count();
                    foreach (var item in messageList)
                        if (item.UserId == myUser.Id)
                        {
                            item.Read = true;
                            db.Entry(item).State = EntityState.Modified;
                            db.SaveChanges();
                        }
                }
            }
            else
                messageList = null;
            //Role role = RoleManager.Roles.Single(r => r.Name == "direktor");
            ChatViewModel viewModel = new()
            {
                Users = db.Users.Include(u => u.Position).Include(u => u.Messages).OrderByDescending(u => u.Messages.OrderByDescending(u => u.When).FirstOrDefault().When).ToList(),
                Messages = messageList,
                SelectUser = user,
                ReadMessage = db.Messages.Where(u => u.UserId == myUser.Id && !u.Read && !u.PersonalMessage).ToList()
            };
            ViewBag.Page = page;
            ViewBag.UserName = User.Identity.Name;
            return View(viewModel);
        }

        //Send message to user
        [HttpPost]
        public ActionResult SendMessage(Message message, HttpPostedFileBase[] uploadChat)
        {
            if (uploadChat[0] != null)
            {
                foreach (var file in uploadChat)
                {
                    byte[] imageData = null;
                    using (var binaryReader = new BinaryReader(file.InputStream))
                        imageData = binaryReader.ReadBytes(file.ContentLength);
                    message.FileString = imageData;
                    message.Format = "docx";
                    message.Text = file.FileName;
                    message.UserName = User.Identity.Name;
                    message.UserId = message.UserId;
                    user = db.Users.Find(message.UserId);
                    User myUser = UserManager.FindByEmail(User.Identity.Name);
                    if (user.Id == myUser.Id)
                        message.PersonalMessage = true;
                    else
                        message.PersonalMessage = false;
                    db.Messages.Add(message);
                    db.SaveChanges();
                }
                return RedirectToAction("ChatAccount", new { userId = message.UserId });
            }
            else
            {
                if (ModelState.IsValid)
                {
                    message.Text = trimmerspace.Replace(message.Text, " ").Trim();
                    message.Format = "txt";
                    message.UserName = User.Identity.Name;
                    message.UserId = message.UserId;
                    user = db.Users.Find(message.UserId);
                    User myUser = UserManager.FindByEmail(User.Identity.Name);
                    if (user.Id == myUser.Id)
                        message.PersonalMessage = true;
                    else
                        message.PersonalMessage = false;
                    db.Messages.Add(message);
                    db.SaveChanges();
                    return RedirectToAction("ChatAccount", new { userId = message.UserId });
                }
            }
            return RedirectToAction("ChatAccount", new { userId = message.UserId });
        }

        //Download sender file
        [HttpGet]
        public ActionResult DownloadFile(int? messageId)
        {
            Message message = db.Messages.Find(messageId);
            if (message == null)
                return RedirectToAction("Error", "Home");
            return File(message.FileString, MediaTypeNames.Application.Octet, message.Text);
        }

        //Delete message
        [HttpGet]
        public ActionResult DeleteMessage(int? messageId)
        {
            Message message = db.Messages.Find(messageId);
            if (message == null)
                return RedirectToAction("Error", "Home");
            var id = message.UserId;
            db.Messages.Remove(message);
            db.SaveChanges();
            return RedirectToAction("ChatAccount", new { userId = id });
        }
    }
}
