using Diploma_project.App_Data;
using Diploma_project.Models;
using Diploma_project.ViewModels;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using System.Collections.Generic;
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
       //.Where(u => u.Roles.Any(r => r.RoleId != role.Id))
        readonly PortalContext db = new();
        User user;
        List<Message> messageList;
        readonly Regex trimmerspace = new(@"\s\s+");
        private ApplicationUserManager UserManager { get => HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>(); }
        private ApplicationRoleManager RoleManager { get => HttpContext.GetOwinContext().GetUserManager<ApplicationRoleManager>(); }

        //Show users chat
        [HttpGet]
        public ActionResult ChatAccount(string userId)
        {
            if (userId != null)
            {
                user = db.Users.Find(userId);
                User myUser = UserManager.FindByEmail(User.Identity.Name);
                if (user.Id == myUser.Id)
                    messageList = db.Messages.Where(u => u.UserName == myUser.UserName && u.UserId == myUser.Id).Where(u => u.PersonalMessage).ToList();
                else
                    messageList = db.Messages.Where(u => u.UserName == user.UserName || u.UserName == myUser.UserName && u.UserId == myUser.Id || u.UserId == user.Id).Where(u => !u.PersonalMessage).ToList();
            }
            else
                messageList = null;
            Role role = RoleManager.Roles.Single(r => r.Name == "direktor");
            ChatViewModel viewModel = new()
            {
                Users = db.Users.ToList(),
                Messages = messageList,
                SelectUser = db.Users.Find(userId)
            };
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
